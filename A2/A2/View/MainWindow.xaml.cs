using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    public Vehicles Vehicles;
    public Journeys Journeys;
    public Services Services;
    public FuelPurchases FuelPurchases;
    public Database Database;

    Vehicle selectedVehicle;

    public bool Online = false;

    public MainWindow()
    {
      InitializeComponent();
      SetVehiclePropertyButtons( false );

      // Set default state to btnView to prevent view from receiving a null value;
      btnView.IsEnabled = false;

      // Instantiate collections
      Vehicles = new Vehicles();
      Journeys = new Journeys();
      Services = new Services();
      FuelPurchases = new FuelPurchases();

      // Test database connection
      if ( InstantiateDatabase() )
      {
        Online = true;

        tbStatus.Text = "Connected to database (yay!)";
        tbStatus.Foreground = Brushes.DarkGreen;

        // Load collections, query from database and store values.
        LoadCollections();
      }
      else
      {
        // Load collections from stored JSON (if any)
        GoOffline();
      }
    }

    /// <summary>
    /// Instantiates connection to the database and tests it
    /// </summary>
    /// <returns>True if successful, false otherwise.</returns>
    private bool InstantiateDatabase()
    {
      Database = new Database("nmt_fleet_manager", "nmt_fleet_manager", "Fleet2019S2");

      if ( Database.LastException != null )
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Run necessary routine to initiate offline mode
    /// </summary>
    private void GoOffline()
    {
      MessageBox.Show(
        "Could not connect to database. Will use last data retrieved instead with viewing capabilities only.",
        "Database Error",
        MessageBoxButton.OK,
        MessageBoxImage.Exclamation
      );

      // Load data resources into collections
      Vehicles.Load(new Offline());

      Journeys.Load(Vehicles.List, new Offline());
      Services.Load(Vehicles.List, new Offline());
      FuelPurchases.Load(Vehicles.List, new Offline());

      btnAdd.IsEnabled = false;
      tbStatus.Text = "Offline Mode: View Only";
      tbStatus.Foreground = Brushes.Red;

      DataContext = Vehicles;
    }

    /// <summary>
    /// Passes database resources and backup offline mode to collections.
    /// </summary>
    private void LoadCollections()
    {
      Vehicles.Load(new Offline(), Database);

      Journeys.Load(Vehicles.List, new Offline(), Database);
      Services.Load(Vehicles.List, new Offline(), Database);
      FuelPurchases.Load(Vehicles.List, new Offline(), Database);

      DataContext = Vehicles;
    }

    /// <summary>
    /// Refreshes all information shown on the window.
    /// </summary>
    public void Refresh()
    {
      string selectedVehicleOdometer = "-";

      if (lvVehicles.SelectedItem is Vehicle)
      {
        // Set selected vehicle in the list and propagate context to groupboxes.
        selectedVehicle = (Vehicle)lvVehicles.SelectedItem;
        CollectionViewSource.GetDefaultView(lvVehicles.ItemsSource).Refresh();

        ObservableCollection<Journey> journeys = Journeys.ByVehicle(selectedVehicle);
        ObservableCollection<Service> services = Services.ByVehicle(selectedVehicle);
        ObservableCollection<FuelPurchase> fuelPurchases = FuelPurchases.ByVehicle(selectedVehicle);

        gbVehicle.DataContext = selectedVehicle;
        
        // Consequence of not having MVVM: have to null DataContext and assign the expected DataContext for changes to mitigate
        gbJourney.DataContext = null;
        gbJourney.DataContext = journeys.Count > 0 ? journeys[0] : null;

        gbRecentService.DataContext = null;
        gbRecentService.DataContext = services.Count > 0 ? services[0] : null;

        gbFuelPurchase.DataContext = null;
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
      if ( Online )
      {
        VehicleWindow vehicleWindow = new VehicleWindow(Journeys, FuelPurchases, Services, true)
        {
          Owner = this,
          DataContext = new Vehicle(Vehicles.FindId())
        };

        vehicleWindow.ShowDialog();
      } else
      {
        MessageBox.Show("Limited 'View Only' Capabilities on Offline Mode");
      }
    }

    private void Window_Closed( object sender, EventArgs e )
    {
      // Try closing database connections (if any)
      try
      {
        Database.Close();
      }
      catch ( Exception )
      {
        // TODO: Nothing
      }
      finally
      {
        // Always save current state to offline storage.
        Vehicles.SaveOffline();
        
        FuelPurchases.SaveOffline();
        Journeys.SaveOffline();
        Services.SaveOffline();
      }
    }
  }
}
