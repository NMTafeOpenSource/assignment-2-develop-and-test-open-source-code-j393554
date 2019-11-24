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

    /// <summary>
    /// Load fuel purchases from database;
    /// </summary>
    public void Load()
    {
      // TODO: Load Journeys with SQL Statement
    }

    /// <summary>
    /// Add a service into the collection
    /// </summary>
    /// <param name="vehicle">Vehicle to be referenced on this service</param>
    /// <param name="odometer">The odometer of the vehicle when serviced</param>
    /// <param name="cost">Cost of service. Up to 2 decimal points</param>
    /// <param name="date">Date of service</param>
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

    /// <summary>
    /// Edit an existing service
    /// </summary>
    /// <param name="service">The service to be referenced and to be modified</param>
    /// <param name="odometer">The odometer of the vehicle when serviced</param>
    /// <param name="cost">Cost of service. Up to 2 decimal points</param>
    /// <param name="date">Date of service</param>
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

    /// <summary>
    /// Retrieves the most recent service
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve the most recent service</param>
    /// <returns>The most recent service</returns>
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

    /// <summary>
    /// Retrieves services by a supplied vehicle.
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve all services</param>
    /// <returns>An ObservableCollection of the services by the vehicle passed</returns>
    public ObservableCollection<Service> ByVehicle( Vehicle vehicle )
    {
      ObservableCollection<Service> vehicleServices = new ObservableCollection<Service>(
        List
          .Where(i => i.VehicleId == vehicle.Id)
          .OrderBy(i => i.Odometer)
          .Reverse()
          .ToList()
      );

      return vehicleServices;
    }

    /// <summary>
    /// Finds the most recent and suitable ID to be used for a new item.
    /// </summary>
    /// <returns>Most suitable ID to be used next for a new item.</returns>
    public int FindId()
    {
      int id;
      int tail;

      // Sort list by ID
      ObservableCollection<Service> listSortedID = new ObservableCollection<Service>(
        List.OrderBy(j => j.Id).ToList()
      );

      tail = listSortedID.Count;

      if (tail == 0)
      {
        id = 1;
      }
      else
      {
        tail--;
        id = listSortedID[tail].Id + 1;
      }

      return id;
    }
  }
}
