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
    readonly FuelPurchases fuelPurchases;
    ObservableCollection<FuelPurchase> filtered;
    FuelPurchase selected;
    readonly Vehicle vehicle;
    bool addMode = false, editMode = false, changes = false, isParentMain = true, online = false;
    List<Control> fields;

    public FuelPurchasesWindow( FuelPurchases fuelPurchases, Vehicle vehicle, bool isParentMain = true )
    {
      InitializeComponent();

      this.vehicle = vehicle;
      this.fuelPurchases = fuelPurchases;
      this.isParentMain = isParentMain;

      CollateFields();
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

      tbLitres.IsEnabled = enable;
      tbCost.IsEnabled = enable;
      dpDate.IsEnabled = enable;

      btnSave.IsEnabled = false;
      btnDelete.IsEnabled = false;
    }

    /// <summary>
    /// Enables Edit mode, blocking out buttons, fields and blanking them. Can be toggled.
    /// </summary>
    /// <param name="enable"></param>
    private void ToggleEditMode( bool enable )
    {
      tbLitres.IsEnabled = enable;
      tbCost.IsEnabled = enable;
      dpDate.IsEnabled = enable;

      btnSave.IsEnabled = enable;
      btnDelete.IsEnabled = enable;

      editMode = enable;
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

    /// <summary>
    /// Collate fields for validation later
    /// </summary>
    private void CollateFields()
    {
      fields = new List<Control>( new Control[] {
        tbLitres,
        tbCost,
        dpDate
      } );
    }

    /// <summary>
    /// Based on the collated fields and defined rules in the function, validates input
    /// </summary>
    /// <returns>Returns true if successful, false otherwise and shows failed validation indicators</returns>
    private bool ValidateFields()
    {
      bool valid = true;
      int anyNumberMaxValue = 100000;

      foreach (Control field in fields)
      {
        field.ClearValue(BorderBrushProperty); // Clear visible failed validation indicator
        bool fieldFailedValidation = false;

        if ( field is DatePicker castedDatePicker )
        {
          if ( castedDatePicker.SelectedDate == null )
          {
            fieldFailedValidation = true;
          }
        }
        else
        {
          TextBox castedField = (TextBox)field;

          if (castedField.Name == "tbLitres" || castedField.Name == "tbCost")
          {
            if ( castedField.Text == "" )
            {
              fieldFailedValidation = true;
            }
            else if ( ! decimal.TryParse(castedField.Text, out decimal d))
            {
              fieldFailedValidation = true;
            }
            else if ( decimal.Parse( castedField.Text ) > anyNumberMaxValue)
            {
              fieldFailedValidation = true;
            }
          }
        }

        if ( fieldFailedValidation )
        {
          valid = false;
          field.BorderBrush = Brushes.Red;
        }
      }

      return valid;
    }

    private void LvItems_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      if (lvItems.SelectedItem is FuelPurchase)
      {
        if ( online )
        {
          if (!editMode)
          {
            ToggleEditMode(true);
          }
        }
        else {
          foreach ( Control field in fields )
          {
            field.IsEnabled = false;
          }
        }

        selected = (FuelPurchase)lvItems.SelectedItem;
        SetSelectedToFields();
      }
    }

    private void BtnAdd_Click( object sender, RoutedEventArgs e )
    {
      if ( editMode )
      {
        ToggleEditMode( false );
      }

      if (addMode)
      {
        if ( ValidateFields() )
        {
          fuelPurchases.Add(
            vehicle,
            decimal.Parse(tbCost.Text),
            double.Parse(tbLitres.Text),
            (DateTime)dpDate.SelectedDate
          );

          changes = true;
          ToggleAddMode( false );
          Refresh();
        }
      }
      else
      {
        ToggleAddMode( true );
      }
    }

    private void BtnCancel_Click( object sender, RoutedEventArgs e )
    {
      ToggleAddMode(false);
    }

    private void BtnSave_Click( object sender, RoutedEventArgs e )
    {
      if ( ValidateFields() )
      {
        fuelPurchases.Edit(selected, decimal.Parse(tbCost.Text), double.Parse(tbLitres.Text), (DateTime)dpDate.SelectedDate);
        Refresh();

        changes = true;
        
        lvItems.UnselectAll();
        SetSelectedToFields( true );

        if ( addMode )
        {
          ToggleAddMode(false);

        }
        else
        {
          ToggleEditMode(false);
        }
      }
    }

    private void BtnDelete_Click( object sender, RoutedEventArgs e )
    {
      fuelPurchases.Delete(selected);
      SetSelectedToFields(true);
      ToggleEditMode(false);
      Refresh();
      changes = true;
    }
    
    private void Window_Loaded( object sender, RoutedEventArgs e )
    {
      Refresh();

      online = Owner is MainWindow ? ((MainWindow)Owner).Online : ((VehicleWindow)Owner).Online;

      ToggleAddMode( false );
      ToggleEditMode( false );

      btnAdd.IsEnabled = online ? true : false;
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
          ((VehicleWindow)Owner).Refresh(true);
        }
      }
    }
  }
}
