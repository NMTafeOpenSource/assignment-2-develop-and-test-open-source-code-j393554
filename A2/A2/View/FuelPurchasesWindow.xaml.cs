using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
  /// Interaction logic for FuelPurchasesWindow.xaml
  /// </summary>
  public partial class FuelPurchasesWindow : Window
  {
    FuelPurchases fuelPurchases;
    ObservableCollection<FuelPurchase> filtered;
    FuelPurchase selected;
    readonly Vehicle vehicle;
    bool addMode = false, editMode = false, changes = false, isParentMain = true;

    public FuelPurchasesWindow( FuelPurchases fuelPurchases, Vehicle vehicle, bool isParentMain = true )
    {
      InitializeComponent();

      this.vehicle = vehicle;
      this.fuelPurchases = fuelPurchases;
      this.isParentMain = isParentMain;
    }

    /// <summary>
    /// Refreshes list by re-grouping items that belong to vehicle and refresh list view.
    /// </summary>
    private void Refresh()
    {
      filtered = fuelPurchases.ByVehicle(vehicle);
      lvItems.ItemsSource = filtered;
      CollectionViewSource.GetDefaultView(lvItems.ItemsSource).Refresh();
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

        SetSelectedToFields(true);
      }
      else
      {
        lvItems.IsEnabled = true;

        btnCancel.Visibility = Visibility.Hidden;

        SetSelectedToFields(true);
      }

      ToggleEditFields(enable);
    }

    /// <summary>
    /// Enables Edit mode, blocking out buttons, fields and blanking them. Can be toggled.
    /// </summary>
    /// <param name="enable"></param>
    private void ToggleEditFields( bool enable )
    {
      tbLitres.IsEnabled = enable;
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
        tbLitres.Text = null;
        tbCost.Text = null;
        dpDate.SelectedDate = null;
      }
      else
      {
        tbLitres.Text = selected.Litres.ToString();
        tbCost.Text = selected.Cost.ToString();
        dpDate.SelectedDate = selected.Date;
      }
    }

    private void LvItems_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      if (lvItems.SelectedItem is FuelPurchase)
      {
        if (!editMode)
        {
          editMode = true;

          btnSave.IsEnabled = true;
          btnDelete.IsEnabled = true;
          ToggleEditFields(true);
        }

        selected = (FuelPurchase)lvItems.SelectedItem;
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
        fuelPurchases.Add(
          vehicle,
          decimal.Parse(tbCost.Text),
          double.Parse(tbLitres.Text),
          (DateTime)dpDate.SelectedDate
        );

        changes = true;

        // Refresh list
        Refresh();
      }

      ToggleAddMode(addMode ? false : true);
    }

    private void BtnCancel_Click( object sender, RoutedEventArgs e )
    {
      ToggleAddMode(false);
    }

    private void BtnSave_Click( object sender, RoutedEventArgs e )
    {
      fuelPurchases.Edit(selected, decimal.Parse(tbCost.Text), double.Parse(tbLitres.Text), (DateTime)dpDate.SelectedDate);
      Refresh();
      changes = true;
    }

    private void BtnDelete_Click( object sender, RoutedEventArgs e )
    {
      fuelPurchases.Delete(selected);
      SetSelectedToFields(true);
      ToggleEditFields(false);
      Refresh();
      editMode = false;
      btnSave.IsEnabled = false;
      btnDelete.IsEnabled = false;

      changes = true;
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

    private void Window_Loaded( object sender, RoutedEventArgs e )
    {
      Refresh();
      ToggleAddMode( false );
    }
  }
}
