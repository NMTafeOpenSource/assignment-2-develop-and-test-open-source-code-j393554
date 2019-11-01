using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Services
  {
    public ObservableCollection<Service> List { get; private set; }
    public ObservableCollection<Service> SelectedItem { get; set; }

    public Services()
    {
      List = new ObservableCollection<Service>();
      Load();
    }

    public void Load()
    {
      // TODO: Load Journeys with SQL Statement
    }

    public void Add( Vehicle vehicle, int odometer, decimal cost, DateTime date )
    {
      int id = FindId();

      List.Add(
        new Service( id, vehicle )
        {
          Odometer = odometer,
          Cost = cost,
          Date = date
        }
      );
    }

    public void Edit( Service service, int odometer, decimal cost, DateTime date )
    {
      var FoundService = List.FirstOrDefault(i => i.Id == service.Id);

      if (FoundService != null)
      {
        FoundService.Odometer = odometer;
        FoundService.Cost = cost;
        FoundService.Date = date;
      }
    }

    public void Delete( Service service )
    {
      List.Remove( service );
    }

    public Service Recent( Vehicle vehicle )
    {
      Service RecentService = null;

      if ( List.Count > 0 )
      {
        // Group Journeys by Vehicle ID.
        ObservableCollection<Service> VehicleServices = ByVehicle( vehicle );
      
        if ( VehicleServices.Count > 0 )
        {
          RecentService = VehicleServices[0];
        }
      }

      return RecentService;
    }

    public ObservableCollection<Service> ByVehicle( Vehicle vehicle )
    {
      ObservableCollection<Service> VehicleServices = new ObservableCollection<Service>(
        List
          .Where(i => i.VehicleId == vehicle.Id)
          .OrderBy(i => i.Odometer)
          .Reverse()
          .ToList()
      );

      return VehicleServices;
    }

    public int FindId()
    {
      int id;
      int tail;

      tail = List.Count;

      if (tail == 0)
      {
        id = 1;
      }
      else
      {
        tail--;
        id = List[tail].Id + 1;
      }

      return id;
    }
  }
}
