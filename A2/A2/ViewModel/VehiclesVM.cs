using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  class VehiclesVM : INotifyPropertyChanged
  {
    public ObservableCollection<Vehicle> Vehicles { get; private set; }

    internal void RaisePropertyChanged(string prop)
    {
      if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private object _SelectedVehicle;
    public object SelectedVehicle {
      get
      {
        return _SelectedVehicle;
      }
      set
      {
        if ( _SelectedVehicle != value )
        {
          _SelectedVehicle = value;
          RaisePropertyChanged( "SelectedPerson" );
        }
      }
    }

    public VehiclesVM() {
      Vehicles = new ObservableCollection<Vehicle>
      {
        new Vehicle( "BMW", "X5", 2006, "1BGZ784", 93, 91573 ),
        new Vehicle( "Tesla", "Roadster", 2008, "8HDZ576", 0, 67453 ),
        new Vehicle( "Chevrolet", "Cadillac", 1959, "C4D1LL4C", 79, 87037 )
      };
    }
  }
}