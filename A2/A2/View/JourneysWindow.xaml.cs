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
    Journeys journeys;
    ObservableCollection<Journey> filteredJourneys;
    Journey selectedJourney;
    readonly Vehicle vehicle;
    bool addMode = false, editMode = false, changes = false, isParentMain = true;

    public JourneysWindow( Journeys journeysVM, Vehicle vehicle, bool isParentMain = true )
    {
      InitializeComponent();
      this.vehicle = vehicle;
      this.journeys = journeysVM;
      this.isParentMain = isParentMain;
    }

    /// <summary>
    /// Refreshes list by re-grouping items that belong to vehicle and refresh list view.
    /// </summary>
    private void RefreshList()
    {
      filteredJourneys = journeys.ByVehicle(vehicle);
      lvJourneys.ItemsSource = filteredJourneys;
      CollectionViewSource.GetDefaultView(lvJourneys.ItemsSource).Refresh();
    }

    /// <summary>
    /// Enables Edit mode, blocking out buttons, fields and blanking them. Can be toggled.
    /// </summary>
    /// <param name="enable"></param>
    private void ToggleEditFields( bool enable )
    {
      dtpStartDate.IsEnabled = enable;
      dtpEndDate.IsEnabled = enable;
      tbStartOdometer.IsEnabled = enable;
      tbEndOdometer.IsEnabled = enable;
    }

    /// <summary>
    /// Enables Add Mode, blocking out buttons, fields and blanking them. Can be toggled.
    /// </summary>
    /// <param name="enable"></param>
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

    /// <summary>
    /// On change of selection in list view, update information shown in grouped fields.
    /// </summary>
    /// <param name="empty">True to empty fields, otherwise false to leave alone.</param>
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
      Journey RecentJourney = journeys.recentJourney( vehicle );
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

    private void LvJourneys_SelectionChanged( object sender, SelectionChangedEventArgs e )
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

    private void BtnAdd_Click( object sender, RoutedEventArgs e )
    {
      if (editMode)
      {
        editMode = false;
      }

      if (addMode)
      {
        // TODO: Validate
        journeys.Add(
          vehicle,
          (DateTime)dtpStartDate.Value,
          (DateTime)dtpEndDate.Value,
          int.Parse(tbStartOdometer.Text),
          int.Parse(tbEndOdometer.Text)
        );

        changes = true;

        // Refresh list
        RefreshList();
      }

      ToggleAddMode(addMode ? false : true);
    }
    
    private void BtnSave_Click( object sender, RoutedEventArgs e )
    {
      journeys.EditJourney( selectedJourney, (DateTime) dtpStartDate.Value, (DateTime) dtpEndDate.Value, int.Parse( tbStartOdometer.Text ), int.Parse( tbEndOdometer.Text ) );
      RefreshList();
      changes = true;
    }

    private void BtnDelete_Click( object sender, RoutedEventArgs e )
    {
      journeys.DeleteJourney( selectedJourney );
      SetSelectedToFields( true );
      ToggleEditFields( false );
      RefreshList();
      editMode = false;
      btnSave.IsEnabled = false;
      btnDelete.IsEnabled = false;

      changes = true;
    }
    
    private void BtnCancel_Click( object sender, RoutedEventArgs e )
    {
      ToggleAddMode(false);
    }
    
    private void Window_Loaded( object sender, RoutedEventArgs e )
    {
      RefreshList();
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