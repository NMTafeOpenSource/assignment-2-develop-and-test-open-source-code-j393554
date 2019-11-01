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

    private void RefreshList()
    {
      filtered = fuelPurchases.ByVehicle(vehicle);
      lvItems.ItemsSource = filtered;
      CollectionViewSource.GetDefaultView(lvItems.ItemsSource).Refresh();
    }

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

    private void ToggleEditFields( bool enable )
    {
      tbLitres.IsEnabled = enable;
      tbCost.IsEnabled = enable;
      dpDate.IsEnabled = enable;
    }

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

    private void lvItems_SelectionChanged( object sender, SelectionChangedEventArgs e )
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

    private void btnAdd_Click( object sender, RoutedEventArgs e )
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
        RefreshList();
      }

      ToggleAddMode(addMode ? false : true);
    }

    private void btnCancel_Click( object sender, RoutedEventArgs e )
    {
      ToggleAddMode(false);
    }

    private void btnSave_Click( object sender, RoutedEventArgs e )
    {
      fuelPurchases.Edit(selected, decimal.Parse(tbCost.Text), double.Parse(tbLitres.Text), (DateTime)dpDate.SelectedDate);
      RefreshList();
      changes = true;
    }

    private void btnDelete_Click( object sender, RoutedEventArgs e )
    {
      fuelPurchases.Delete(selected);
      SetSelectedToFields(true);
      ToggleEditFields(false);
      RefreshList();
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
      RefreshList();
      ToggleAddMode( false );
    }
  }
}
