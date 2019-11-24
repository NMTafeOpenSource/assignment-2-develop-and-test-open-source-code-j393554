using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A2
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public Vehicles Vehicles = new Vehicles();
    public Journeys Journeys = new Journeys();
    public Services Services = new Services();
    public FuelPurchases FuelPurchases = new FuelPurchases();

    Vehicle selectedVehicle;

    public MainWindow()
    {
      InitializeComponent();
      DataContext = Vehicles;
      SetVehiclePropertyButtons( false );

      // Set default state to btnView to prevent view from receiving a null value;
      btnView.IsEnabled = false;

      // Seed
      Vehicles.Add("BMW", "X5", 2006, "1BGZ784", 93, 50 );
      Vehicles.Add("Tesla", "Roadster", 2008, "8HDZ576", 0);
      Vehicles.Add("Chevrolet", "Cadillac", 1959, "C4D1LL4C", 79);

      Journeys.Add( Vehicles.List[0], new DateTime(2019, 1, 15, 15, 45, 00), 10 );
      Journeys.Add( Vehicles.List[0], new DateTime(2019, 3, 16, 21, 10, 00), 20 );
      Journeys.Add( Vehicles.List[2], new DateTime(2019, 11, 19, 18, 10, 00), 30 );

      FuelPurchases.Add( Vehicles.List[0], 357.11m, 39.23, new DateTime(2019, 11, 18, 22, 15, 00));
      FuelPurchases.Add( Vehicles.List[0], 161.80m, 49.57, new DateTime(2019, 11, 21, 12, 15, 00));

      Services.Add( Vehicles.List[0], 10, 473.02m, new DateTime(2018, 12, 20) );
      Services.Add( Vehicles.List[0], 15, 537.20m, new DateTime(2019, 3, 16) );
    }

    /// <summary>
    /// Refreshes all information shown on the window.
    /// </summary>
    public void Refresh()
    {
      string selectedVehicleOdometer = "-";

      if (lvVehicles.SelectedItem is Vehicle)
      {
        selectedVehicle = (Vehicle)lvVehicles.SelectedItem;
        CollectionViewSource.GetDefaultView(lvVehicles.ItemsSource).Refresh();

        ObservableCollection<Journey> journeys = Journeys.ByVehicle(selectedVehicle);
        ObservableCollection<Service> services = Services.ByVehicle(selectedVehicle);
        ObservableCollection<FuelPurchase> fuelPurchases = FuelPurchases.ByVehicle(selectedVehicle);

        gbVehicle.DataContext = selectedVehicle;
        gbJourney.DataContext = journeys.Count > 0 ? journeys[0] : null;
        gbRecentService.DataContext = services.Count > 0 ? services[0] : null;
        gbFuelPurchase.DataContext = fuelPurchases.Count > 0 ? fuelPurchases[0] : null;

        selectedVehicleOdometer = Vehicles.TotalDistance(journeys).ToString();

        SetVehiclePropertyButtons();
      }
      else
      {
        gbJourney.DataContext = null;
        SetVehiclePropertyButtons(false);
      }

      tbOdometer.Text = selectedVehicleOdometer;
    }

    /// <summary>
    /// Sets vehicle property (fuel purchases, journeys, services) buttons to a state. Can be toggled.
    /// </summary>
    /// <param name="enable">True to enable property buttons, otherwise false</param>
    private void SetVehiclePropertyButtons( bool enable = true )
    {
      BtnFuelPurchases.IsEnabled = enable;
      BtnJourneys.IsEnabled = enable;
      BtnServices.IsEnabled = enable;
    }

    private void BtnJourneys_Click(object sender, RoutedEventArgs e)
    {
      JourneysWindow JourneysWindow;

      JourneysWindow = new JourneysWindow( Journeys, selectedVehicle )
      {
        Owner = this,
      };

      JourneysWindow.ShowDialog();
    }

    private void BtnFuelPurchases_Click( object sender, RoutedEventArgs e )
    {
      FuelPurchasesWindow FuelPurchasesWindow = new FuelPurchasesWindow( FuelPurchases, selectedVehicle )
      {
        Owner = this
      };
      FuelPurchasesWindow.ShowDialog();
    }

    private void BtnServices_Click( object sender, RoutedEventArgs e )
    {
      ServicesWindow ServicesWindow = new ServicesWindow( Services, selectedVehicle )
      {
        Owner = this
      };
      ServicesWindow.ShowDialog();
    }

    private void LvVehicles_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      Refresh();
      btnView.IsEnabled = true;
    }

    private void BtnView_Click( object sender, RoutedEventArgs e )
    {
      VehicleWindow vehicleWindow = new VehicleWindow( Journeys, FuelPurchases, Services, false )
      {
        Owner = this,
        DataContext = selectedVehicle
      };

      vehicleWindow.ShowDialog();
    }

    private void BtnAdd_Click( object sender, RoutedEventArgs e )
    {
      VehicleWindow vehicleWindow = new VehicleWindow(Journeys, FuelPurchases, Services, true)
      {
        Owner = this,
        DataContext = new Vehicle( Vehicles.FindId() )
      };

      vehicleWindow.ShowDialog();
    }
  }
}
