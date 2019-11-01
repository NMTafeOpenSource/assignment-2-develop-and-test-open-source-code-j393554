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
using System.Windows.Shapes;

namespace A2
{
  /// <summary>
  /// Interaction logic for Vehicle.xaml
  /// </summary>
  public partial class VehicleWindow : Window
  {
    Vehicle vehicle;

    Vehicles vehicles;
    Journeys journeys;
    FuelPurchases fuelPurchases;
    Services services;
    
    ObservableCollection<Journey> journeysList;
    ObservableCollection<FuelPurchase> fuelPurchasesList;
    ObservableCollection<Service> servicesList;

    bool addMode = false, editMode = false, changes = false;

    public VehicleWindow( Journeys journeys, FuelPurchases fuelPurchases, Services services, bool addMode = true )
    {
      InitializeComponent();

      this.journeys = journeys;
      this.fuelPurchases = fuelPurchases;
      this.services = services;

      tbOdometerCurrent.IsEnabled = false;

      if ( addMode )
      {
        this.addMode = true;
      }
      else
      {
        editMode = true;
      }
    }

    public void Refresh( bool metaChanged = false )
    {
      SetDataContexts();
      UpdateOdometer();
      SetNextService();
      SetFuelEfficiency();

      tbName.GetBindingExpression( TextBlock.TextProperty ).UpdateTarget();

      if ( metaChanged )
      {
        changes = true;
      }
    }

    private void UpdateOdometer()
    {
      int odometer = CalculateDistanceTraveled();
      tbOdometer.Text = odometer.ToString() + " KM(s)";
      tbOdometerCurrent.Text = odometer.ToString();
    }

    private int CalculateDistanceTraveled()
    {
      int odometer;
      odometer = vehicle.TotalDistanceTraveled(journeysList);
      return odometer;
    }

    private void ToggleAddMode( bool enable = true )
    {
      addMode = enable;

      btnJourneys.IsEnabled = !enable;
      btnFuelPurchases.IsEnabled = !enable;
      btnServices.IsEnabled = !enable;
      btnEdit.IsEnabled = !enable;
      btnDelete.IsEnabled = !enable;

      btnSave.IsEnabled = enable;

      if (enable)
      {
        tbManufacturer.Text = null;
        tbModel.Text = null;
        tbMakeYear.Text = null;
        tbRegistrationNumber.Text = null;
        tbTankCapacity.Text = null;
        tbOdometerSaved.Text = null;
      }
    }

    private void ToggleEditMode( bool enable )
    {
      btnJourneys.IsEnabled = ! enable;
      btnFuelPurchases.IsEnabled = ! enable;
      btnServices.IsEnabled = ! enable;

      btnDelete.IsEnabled = ! enable;
      btnEdit.IsEnabled = ! enable;
      btnSave.IsEnabled = enable;

      tbManufacturer.IsEnabled = enable;
      tbModel.IsEnabled = enable;
      tbMakeYear.IsEnabled = enable;
      tbRegistrationNumber.IsEnabled = enable;
      tbTankCapacity.IsEnabled = enable;
      tbOdometerSaved.IsEnabled = enable;
    }

    private void SetDataContexts()
    {
      journeysList = journeys.ByVehicle(vehicle);
      fuelPurchasesList = fuelPurchases.ByVehicle(vehicle);
      servicesList = services.ByVehicle(vehicle);

      if ( journeysList.Count > 0 )
      {
        gbJourney.DataContext = journeysList[0];
      }

      if ( fuelPurchasesList.Count > 0 )
      {
        gbFuelPurchase.DataContext = fuelPurchasesList[0];
      }

      if ( servicesList.Count > 0 )
      {
        gbRecentService.DataContext = servicesList[0];
      }
    }

    private void SetNextService()
    {
      int gap = vehicles.TravelServiceGap( vehicle, servicesList, journeysList );
      string nextService;
      

      if ( servicesList.Count == 0 )
      {
        // A vehicle can have no service, yet.
        nextService = "No service information; New Vehicle";
        
        // But a vehicle that has no service information but its odometer is past it's service limit
        if ( vehicle.Odometer > vehicle.ServiceLimit )
        {
          // Then it's due for a service.
          nextService = "NOW";
        }
      }
      else if ( gap >= vehicle.ServiceLimit )
      {
        // Due for a service
        nextService = "NOW";
      }
      else
      {
        // Not yet due, service in next n-KM(s)
        nextService = "next " + (vehicle.ServiceLimit - gap).ToString() + " KM(s)";
      }

      tbNextService.Text = nextService;
    }

    private void SetFuelEfficiency()
    {
      string fuelEfficiencyText = "Efficiency too low (Below 1KM/L)";

      int totalOdometer = vehicles.TotalDistance( vehicle, journeysList );
      double totalFuelLitres = vehicles.TotalFuelLitres( fuelPurchasesList );

      double fuelEfficiency = totalOdometer / totalFuelLitres;

      if ( totalFuelLitres == 0 )
      {
        fuelEfficiencyText = "No fuel purchase information";
      }
      else if ( fuelEfficiency > 1 )
      {
        fuelEfficiencyText = Math.Round( fuelEfficiency, 2 ) + " KM(s) / L";
      }

      tbFuelEfficiency.Text = fuelEfficiencyText;
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      vehicles.Delete( vehicle );
      changes = true;
      Close();
    }

    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if ( addMode )
      {
        vehicles.Add(
          tbManufacturer.Text,
          tbModel.Text,
          int.Parse(tbMakeYear.Text),
          tbRegistrationNumber.Text,
          double.Parse(tbTankCapacity.Text),
          int.Parse(tbOdometerSaved.Text)
        );

        vehicle = vehicles.List.Last();
        DataContext = vehicle;

        ToggleEditMode(false);
        Refresh(true);
      }
      else
      {
        vehicles.Edit( (Vehicle) DataContext );
        vehicle = (Vehicle) DataContext;
        editMode = false;
        
        ToggleEditMode(false);
        Refresh(true);
      }
    }

    private void btnEdit_Click( object sender, RoutedEventArgs e )
    {
      editMode = !editMode;

      ToggleEditMode(editMode);
    }

    private void btnJourneys_Click( object sender, RoutedEventArgs e )
    {
      JourneysWindow JourneysWindow;

      JourneysWindow = new JourneysWindow( journeys, vehicle, false )
      {
        Owner = this,
      };

      JourneysWindow.ShowDialog();
    }

    private void btnFuelPurchases_Click( object sender, RoutedEventArgs e )
    {
      FuelPurchasesWindow FuelPurchasesWindow;

      FuelPurchasesWindow = new FuelPurchasesWindow( fuelPurchases, vehicle, false )
      {
        Owner = this,
      };

      FuelPurchasesWindow.ShowDialog();
    }

    private void btnServices_Click( object sender, RoutedEventArgs e )
    {
      ServicesWindow ServicesWindow;

      ServicesWindow = new ServicesWindow( services, vehicle, journeys, false )
      {
        Owner = this,
      };

      ServicesWindow.ShowDialog();
    }

    private void Window_Loaded( object sender, RoutedEventArgs e )
    {
      vehicle = (Vehicle) DataContext;
      vehicles = ((MainWindow)Owner).Vehicles;
      Refresh();

      if (addMode)
      {
        ToggleAddMode();
      }
      else
      {
        /**
         * We already know we're viewing a vehicle when neither addMode or editMode (implied) was
         * set to true, so we're inverting (turning off) editMode since we're not really editing.
         */
        editMode = ! editMode;
        ToggleEditMode( editMode );
      }
    }

    private void Window_Closed( object sender, EventArgs e )
    {
      // Refresh MainWindow if there are changes
      if ( changes )
      {
        ( ( MainWindow ) Owner ).Refresh();
      }
    }
  }
}
