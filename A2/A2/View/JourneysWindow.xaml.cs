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
    readonly Journeys journeys;
    ObservableCollection<Journey> filteredJourneys;
    Journey selected;
    readonly Vehicle vehicle;
    bool addMode = false, editMode = false, changes = false, isParentMain = true;
    List<Control> fields;

    public JourneysWindow( Journeys journeys, Vehicle vehicle, bool isParentMain = true )
    {
      InitializeComponent();
      this.vehicle = vehicle;
      this.journeys = journeys;
      this.isParentMain = isParentMain;

      CollateFields();
    }

    /// <summary>
    /// Refreshes list by re-grouping items that belong to vehicle and refresh list view.
    /// </summary>
    private void Refresh()
    {
      filteredJourneys = journeys.ByVehicle(vehicle);
      lvJourneys.ItemsSource = filteredJourneys;
      CollectionViewSource.GetDefaultView(lvJourneys.ItemsSource).Refresh();
    }

    /// <summary>
    /// Enables Edit mode, blocking out buttons, fields and blanking them. Can be toggled.
    /// </summary>
    /// <param name="enable"></param>
    private void ToggleEditMode( bool enable )
    {
      dtpDate.IsEnabled = enable;
      tbDistance.IsEnabled = enable;

      btnSave.IsEnabled = enable;
      btnDelete.IsEnabled = enable;

      editMode = enable;
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
      }
      else
      {
        lvJourneys.IsEnabled = true;

        btnCancel.Visibility = Visibility.Hidden;

        SetSelectedToFields( true );
      }

      tbDistance.IsEnabled = enable;
      dtpDate.IsEnabled = enable;

      btnSave.IsEnabled = false;
      btnDelete.IsEnabled = false;
    }

    /// <summary>
    /// On change of selection in list view, update information shown in grouped fields.
    /// </summary>
    /// <param name="empty">True to empty fields, otherwise false to leave alone.</param>
    private void SetSelectedToFields( bool empty = false )
    {
      if ( empty )
      {
        dtpDate.Value = null;
        tbDistance.Text = null;
      }
      else
      {
        dtpDate.Value = selected.Date;
        tbDistance.Text = selected.Distance.ToString();
      }
    }

    /// <summary>
    /// Collate fields for validation later
    /// </summary>
    private void CollateFields()
    {
      fields = new List<Control>(new Control[] {
        tbDistance,
        dtpDate,
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

        // Integers
        if (field.Name == "tbDistance")
        {
          TextBox castedField = (TextBox)field;

          if (castedField.Text == "")
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
        else if ( field is Xceed.Wpf.Toolkit.DateTimePicker castedField )
        {
          // Check for null values
          if (castedField.Value == null)
          {
            fieldFailedValidation = true;
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

    private void LvJourneys_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      if ( lvJourneys.SelectedItem is Journey )
      {
        if ( ! editMode )
        {
          ToggleEditMode( true );
        }

        selected = ( Journey ) lvJourneys.SelectedItem;
        SetSelectedToFields();
      }
    }

    private void BtnAdd_Click( object sender, RoutedEventArgs e )
    {
      if ( editMode )
      {
        ToggleEditMode( false );
      }

      if ( addMode )
      {
        if ( ValidateFields() )
        {
          journeys.Add(
            vehicle,
            (DateTime)dtpDate.Value,
            int.Parse(tbDistance.Text)
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
    
    private void BtnSave_Click( object sender, RoutedEventArgs e )
    {
      if (ValidateFields())
      {
        journeys.Edit( selected, (DateTime) dtpDate.Value, int.Parse( tbDistance.Text ) );
        Refresh();
        changes = true;

        lvJourneys.UnselectAll();
        SetSelectedToFields( true );

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
      journeys.Delete( selected );
      SetSelectedToFields( true );
      ToggleEditMode( false );
      Refresh();
      changes = true;
    }
    
    private void BtnCancel_Click( object sender, RoutedEventArgs e )
    {
      ToggleAddMode(false);
    }
    
    private void Window_Loaded( object sender, RoutedEventArgs e )
    {
      Refresh();
      ToggleAddMode(false);
      ToggleEditMode(false);
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