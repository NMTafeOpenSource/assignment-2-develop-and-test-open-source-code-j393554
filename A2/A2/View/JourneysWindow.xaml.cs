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
  /// Interaction logic for JourneysWindow.xaml
  /// </summary>
  public partial class JourneysWindow : Window
  {
    Journeys journeysVM;
    ObservableCollection<Journey> filteredJourneys;
    Journey selectedJourney;
    readonly Vehicle vehicle;
    bool addMode = false, editMode = false, changes = false, isParentMain = true;

    public JourneysWindow( Journeys journeysVM, Vehicle vehicle, bool isParentMain = true )
    {
      InitializeComponent();
      this.vehicle = vehicle;
      this.journeysVM = journeysVM;
      this.isParentMain = isParentMain;
    }

    private void RefreshJourneyList()
    {
      filteredJourneys = journeysVM.ByVehicle(vehicle);
      lvJourneys.ItemsSource = filteredJourneys;
      CollectionViewSource.GetDefaultView(lvJourneys.ItemsSource).Refresh();
    }

    private void ToggleEditFields( bool enable )
    {
      dtpStartDate.IsEnabled = enable;
      dtpEndDate.IsEnabled = enable;
      tbStartOdometer.IsEnabled = enable;
      tbEndOdometer.IsEnabled = enable;
    }

    private void ToggleAddMode( bool enable )
    {
      addMode = enable;

      // Enable AddMode: Disables and deselects list
      if (addMode)
      {
        lvJourneys.UnselectAll();
        lvJourneys.IsEnabled = false;

        btnCancel.Visibility = Visibility.Visible;

        SetSelectedToFields( true );
        PrefillNewJourneyFields();
      }
      else
      {
        lvJourneys.IsEnabled = true;

        btnCancel.Visibility = Visibility.Hidden;

        SetSelectedToFields( true );
      }

      ToggleEditFields( enable );
    }

    private void SetSelectedToFields( bool empty = false )
    {
      if ( empty )
      {
        dtpStartDate.Value = null;
        dtpEndDate.Value = null;
        tbStartOdometer.Text = null;
        tbEndOdometer.Text = null;
      }
      else
      {
        dtpStartDate.Value = selectedJourney.StartDate;
        dtpEndDate.Value = selectedJourney.EndDate;
        tbStartOdometer.Text = selectedJourney.StartOdometer.ToString();
        tbEndOdometer.Text = selectedJourney.EndOdometer.ToString();
      }
    }

    private void PrefillNewJourneyFields()
    {
      Journey RecentJourney = journeysVM.RecentJourney( vehicle );
      string Odometer;
      
      if ( RecentJourney != null )
      {
        Odometer = RecentJourney.EndOdometer.ToString();
      }
      else
      {
        Odometer = vehicle.Odometer.ToString();
      }

      tbStartOdometer.Text = Odometer;
      tbEndOdometer.Text = Odometer;
    }

    private void lvJourneys_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      if ( lvJourneys.SelectedItem is Journey )
      {
        if ( ! editMode )
        {
          editMode = true;

          btnSave.IsEnabled = true;
          btnDelete.IsEnabled = true;
          ToggleEditFields( true );
        }

        selectedJourney = ( Journey ) lvJourneys.SelectedItem;
        SetSelectedToFields();

      }
    }

    private void btnAdd_Click( object sender, RoutedEventArgs e )
    {
      if (editMode)
      {
        editMode = false;
      }

      if (addMode)
      {
        // TODO: Validate
        journeysVM.Add(
          vehicle,
          (DateTime)dtpStartDate.Value,
          (DateTime)dtpEndDate.Value,
          int.Parse(tbStartOdometer.Text),
          int.Parse(tbEndOdometer.Text)
        );

        changes = true;

        // Refresh list
        RefreshJourneyList();
      }

      ToggleAddMode(addMode ? false : true);
    }
    
    private void btnSave_Click( object sender, RoutedEventArgs e )
    {
      journeysVM.EditJourney( selectedJourney, (DateTime) dtpStartDate.Value, (DateTime) dtpEndDate.Value, int.Parse( tbStartOdometer.Text ), int.Parse( tbEndOdometer.Text ) );
      RefreshJourneyList();
      changes = true;
    }

    private void btnDelete_Click( object sender, RoutedEventArgs e )
    {
      journeysVM.DeleteJourney( selectedJourney );
      SetSelectedToFields( true );
      ToggleEditFields( false );
      RefreshJourneyList();
      editMode = false;
      btnSave.IsEnabled = false;
      btnDelete.IsEnabled = false;

      changes = true;
    }
    
    private void btnCancel_Click( object sender, RoutedEventArgs e )
    {
      ToggleAddMode(false);
    }
    
    private void Window_Loaded( object sender, RoutedEventArgs e )
    {
      RefreshJourneyList();
      ToggleAddMode(false);
    }

    private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
    {
      if (changes)
      {
        // Trigger refresh on owner windows. Only two known paths: MainWindow and VehicleWindow
        if ( isParentMain )
        {
          ((MainWindow)Owner).Refresh();
        }
        else
        {
          ((VehicleWindow)Owner).Refresh( true );
        }
      }
    }
  }
}