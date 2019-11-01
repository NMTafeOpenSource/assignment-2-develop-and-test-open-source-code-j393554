using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2.ViewModels
{
    class VehiclesVM
    {
    private ObservableCollection<Vehicle> _vehicles;

    public ObservableCollection<Vehicle> Vehicles
      {
        get
        {
          return _vehicles;
        }
        set
        {
          _vehicles = value;
          NotifyPropertyChanged("Vehicles");
        }
      }
  }
}
