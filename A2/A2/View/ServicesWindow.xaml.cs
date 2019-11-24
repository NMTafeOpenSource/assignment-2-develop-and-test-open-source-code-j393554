using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace A2
{
  /// <summary>
  /// Interaction logic for ServicesWindow.xaml
  /// </summary>
  public partial class ServicesWindow : Window
  {
    Services services;
    ObservableCollection<Service> filtered;
    Service selected;
    readonly Vehicle vehicle;
    bool addMode = false, editMode = false, changes = false, isParentMain = true;
    List<Control> fields;

    public ServicesWindow( Services services, Vehicle vehicle, bool isParentMain = true )
    {
      InitializeComponent();

      this.vehicle = vehicle;
      this.services = services;
      this.isParentMain = isParentMain;

      CollateFields();
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
      }
      else
      {
        lvItems.IsEnabled = true;

        btnCancel.Visibility = Visibility.Hidden;

        SetSelectedToFields( true );
      }

      tbCost.IsEnabled = enable;
      tbOdometer.IsEnabled = enable;
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
      tbOdometer.IsEnabled = enable;
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

    /// <summary>
    /// Collate fields for validation later
    /// </summary>
    private void CollateFields()
    {
      fields = new List<Control>(new Control[] {
        tbOdometer,
        tbCost,
        dpDate
      });
    }

    /// <summary>
    /// Based on the collated fields and defined rules in the function, validates input
    /// </summary>
    /// <returns>Returns true if successful, false otherwise and shows failed validation indicators</returns>
    private bool ValidateFields()
    {
      bool valid = true;
      int anyNumberMaxValue = 500000;

      foreach (Control field in fields)
      {
        field.ClearValue(BorderBrushProperty); // Clear visible failed validation indicator
        bool fieldFailedValidation = false;

        if ( field is DatePicker castedDatePickerField )
        {
          // Check for null values
          if ( castedDatePickerField.SelectedDate == null )
          {
            fieldFailedValidation = true;
          }
        }
        else
        {
          TextBox castedField = (TextBox)field;

          // Decimals/doubles/floats
          if ( castedField.Name == "tbCost" )
          {
            if ( castedField.Text == "" )
            {
              fieldFailedValidation = true;
            }
            else if (!decimal.TryParse(castedField.Text, out decimal d))
            {
              fieldFailedValidation = true;
            }
            else if (decimal.Parse(castedField.Text) > anyNumberMaxValue)
            {
              fieldFailedValidation = true;
            }
          }
          // Integers
          else if ( castedField.Name == "tbOdometer" )
          {
            if ( castedField.Text == "" )
            {
              fieldFailedValidation = true;
            }
            else if (!int.TryParse(castedField.Text, out int i))
            {
              fieldFailedValidation = true;
            }
            else if (int.Parse(castedField.Text) > anyNumberMaxValue)
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

    private void BtnAdd_Click( object sender, RoutedEventArgs e )
    {
      if (editMode)
      {
        ToggleEditMode(false);
      }

      if (addMode)
      {
        if (ValidateFields())
        {
          services.Add(
            vehicle,
            int.Parse(tbOdometer.Text),
            decimal.Parse(tbCost.Text),
            (DateTime)dpDate.SelectedDate
          );

          changes = true;
          ToggleAddMode(false);
          Refresh();
        }
      }
      else
      {
        ToggleAddMode(true);
      }
    }

    private void BtnSave_Click( object sender, RoutedEventArgs e )
    {
      if (ValidateFields())
      {
        services.Edit(selected, int.Parse(tbOdometer.Text), decimal.Parse(tbCost.Text), (DateTime)dpDate.SelectedDate);
        Refresh();
        changes = true;

        lvItems.UnselectAll();
        SetSelectedToFields(true);

        if (addMode)
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
      services.Delete( selected );
      SetSelectedToFields(true);
      ToggleEditMode( false );
      Refresh();
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
        if ( ! editMode)
        {
          ToggleEditMode(true);
        }

        selected = ( Service ) lvItems.SelectedItem;
        SetSelectedToFields();
      }
    }

    private void Window_Loaded( object sender, RoutedEventArgs e )
    {
      Refresh();
      ToggleAddMode( false );
      ToggleEditMode( false );
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