using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace A2
{
  /// <summary>
  /// Interaction logic for ServicesWindow.xaml
  /// </summary>
  public partial class ServicesWindow : Window
  {
    Services services;
    Journeys journeys;
    ObservableCollection<Service> filtered;
    Service selected;
    readonly Vehicle vehicle;
    bool addMode = false, editMode = false, changes = false, isParentMain = true;

    public ServicesWindow( Services services, Vehicle vehicle, Journeys journeys, bool isParentMain = true )
    {
      InitializeComponent();

      this.vehicle = vehicle;
      this.services = services;
      this.journeys = journeys;
      this.isParentMain = isParentMain;
    }

    /// <summary>
    /// Refreshes list by re-grouping items that belong to vehicle and refresh list view.
    /// </summary>
    private void Refresh()
    {
      filtered = services.ByVehicle( vehicle );
      lvItems.ItemsSource = filtered;
      CollectionViewSource.GetDefaultView( lvItems.ItemsSource ).Refresh();
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
        lvItems.UnselectAll();
        lvItems.IsEnabled = false;

        btnCancel.Visibility = Visibility.Visible;

        SetSelectedToFields( true );
        PrefillNewItemFields();
      }
      else
      {
        lvItems.IsEnabled = true;

        btnCancel.Visibility = Visibility.Hidden;

        SetSelectedToFields( true );
      }

      ToggleEditFields(enable);
    }

    /// <summary>
    /// Prefill's fields based on information of a vehicle or its journeys
    /// </summary>
    private void PrefillNewItemFields()
    {
      Journey recentJourney = journeys.recentJourney( vehicle );
      string Odometer;

      if (recentJourney != null)
      {
        Odometer = recentJourney.EndOdometer.ToString();
      }
      else
      {
        Odometer = vehicle.Odometer.ToString();
      }

      tbOdometer.Text = Odometer;
    }

    /// <summary>
    /// Enables Edit mode, blocking out buttons, fields and blanking them. Can be toggled.
    /// </summary>
    /// <param name="enable"></param>
    private void ToggleEditFields( bool enable )
    {
      tbOdometer.IsEnabled = enable;
      tbCost.IsEnabled = enable;
      dpDate.IsEnabled = enable;
    }

    /// <summary>
    /// On change of selection in list view, update information shown in grouped fields.
    /// </summary>
    /// <param name="empty">True to empty fields, otherwise false to leave alone.</param>
    private void SetSelectedToFields( bool empty = false )
    {
      if (empty)
      {
        tbOdometer.Text = null;
        tbCost.Text = null;
        dpDate.SelectedDate = null;
      }
      else
      {
        tbOdometer.Text = selected.Odometer.ToString();
        tbCost.Text = selected.Cost.ToString();
        dpDate.SelectedDate = selected.Date;
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
        services.Add(
          vehicle,
          int.Parse(tbOdometer.Text),
          decimal.Parse(tbCost.Text),
          (DateTime)dpDate.SelectedDate
        );

        changes = true;

        // Refresh list
        Refresh();
      }

      ToggleAddMode(addMode ? false : true);
    }

    private void BtnSave_Click( object sender, RoutedEventArgs e )
    {
      services.Edit( selected, int.Parse( tbOdometer.Text ), decimal.Parse( tbCost.Text ), ( DateTime ) dpDate.SelectedDate );
      Refresh();
      changes = true;
    }

    private void BtnDelete_Click( object sender, RoutedEventArgs e )
    {
      services.Delete( selected );
      SetSelectedToFields(true);
      ToggleEditFields( false );
      Refresh();
      editMode = false;
      btnSave.IsEnabled = false;
      btnDelete.IsEnabled = false;

      changes = true;
    }

    private void BtnCancel_Click( object sender, RoutedEventArgs e )
    {
      ToggleAddMode( false );
    }

    private void LvItems_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      if (lvItems.SelectedItem is Service)
      {
        if (!editMode)
        {
          editMode = true;

          btnSave.IsEnabled = true;
          btnDelete.IsEnabled = true;
          ToggleEditFields(true);
        }

        selected = ( Service ) lvItems.SelectedItem;
        SetSelectedToFields();
      }
    }

    private void Window_Loaded( object sender, RoutedEventArgs e )
    {
      Refresh();
      ToggleAddMode( false );
    }

    private void Window_Closing( object sender, CancelEventArgs e )
    {
      if (changes)
      {
        // Trigger refresh on owner windows. Only two known paths: MainWindow and VehicleWindow
        if (isParentMain)
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