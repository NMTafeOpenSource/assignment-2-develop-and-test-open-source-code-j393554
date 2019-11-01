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
      Vehicles.Add("BMW", "X5", 2006, "1BGZ784", 93, 91573, 7);
      Vehicles.Add("Tesla", "Roadster", 2008, "8HDZ576", 0, 67453);
      Vehicles.Add("Chevrolet", "Cadillac", 1959, "C4D1LL4C", 79, 87037);

      Journeys.Add( Vehicles.List[0], new DateTime(2019, 1, 15, 11, 30, 00), new DateTime(2019, 1, 15, 15, 45, 00), 92573, 92673 );
      Journeys.Add( Vehicles.List[0], new DateTime(2019, 3, 15, 7, 15, 00), new DateTime(2019, 3, 16, 21, 10, 00), 92900, 92999 );
      Journeys.Add( Vehicles.List[2], new DateTime(2019, 11, 18, 21, 55, 00), new DateTime(2019, 11, 19, 18, 10, 00), 2357, 2937 );

      FuelPurchases.Add( Vehicles.List[0], 357.11m, 999337.327, new DateTime(2019, 11, 18, 22, 15, 00));

      Services.Add( Vehicles.List[0], 92473, 473.02m, new DateTime(2018, 12, 20) );
      Services.Add( Vehicles.List[0], 92950, 537.20m, new DateTime(2019, 3, 16) );
    }

    public void Refresh()
    {
      string selectedVehicleOdometer;

      if (lvVehicles.SelectedItem is Vehicle)
      {
        selectedVehicle = (Vehicle)lvVehicles.SelectedItem;
        ObservableCollection<Journey> journeys = Journeys.ByVehicle(selectedVehicle);
        ObservableCollection<Service> services = Services.ByVehicle(selectedVehicle);
        ObservableCollection<FuelPurchase> fuelPurchases = FuelPurchases.ByVehicle(selectedVehicle);

        gbVehicle.DataContext = selectedVehicle;
        gbJourney.DataContext = journeys.Count > 0 ? journeys[0] : null;
        gbRecentService.DataContext = services.Count > 0 ? services[0] : null;
        gbFuelPurchase.DataContext = fuelPurchases.Count > 0 ? fuelPurchases[0] : null;

        selectedVehicleOdometer = Vehicles.TotalDistance(selectedVehicle, journeys).ToString() + " (" + selectedVehicle.Odometer.ToString() + ")";

        SetVehiclePropertyButtons();
      }
      else
      {
        gbJourney.DataContext = null;
        selectedVehicleOdometer = "-";
        SetVehiclePropertyButtons(false);
      }

      tbOdometer.Text = selectedVehicleOdometer;
    }

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
      ServicesWindow ServicesWindow = new ServicesWindow( Services, selectedVehicle, Journeys )
      {
        Owner = this
      };
      ServicesWindow.ShowDialog();
    }

    private void lvVehicles_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      Refresh();
      btnView.IsEnabled = true;
    }

    private void btnView_Click( object sender, RoutedEventArgs e )
    {
      VehicleWindow vehicleWindow = new VehicleWindow( Journeys, FuelPurchases, Services, false )
      {
        Owner = this,
        DataContext = selectedVehicle
      };

      vehicleWindow.ShowDialog();
    }

    private void btnAdd_Click( object sender, RoutedEventArgs e )
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
