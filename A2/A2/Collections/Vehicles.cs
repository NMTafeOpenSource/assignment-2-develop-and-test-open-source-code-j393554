using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Vehicles
  {
    public ObservableCollection<Vehicle> List { get; private set; }
    public int DefaultVehicleServiceLimit = 10000;

    public Vehicles()
    {
      List = new ObservableCollection<Vehicle>();
      Load();
    }

    /// <summary>
    /// Load vehicles from database;
    /// </summary>
    public void Load()
    {
      // TODO: Load vehicles from SQL/JSON
    }

    /// <summary>
    /// Add a vehicle into the collection
    /// </summary>
    /// <param name="manufacturer">Manufacturer</param>
    /// <param name="model">Model</param>
    /// <param name="makeYear">Make Year</param>
    /// <param name="registrationNumber">Registration Number</param>
    /// <param name="tankCapacity">Tank Capacity. Up to 2 decimal points.</param>
    /// <param name="serviceLimit">Service Limit</param>
    /// <param name="id">(Optional) ID of the vehicle</param>
    public void Add(string manufacturer, string model, int makeYear, string registrationNumber, double tankCapacity, int serviceLimit = 0, int id = 0)
    {
      if ( id == 0 )
      {
        id = FindId();
      }

      if ( serviceLimit == 0 )
      {
        serviceLimit = DefaultVehicleServiceLimit;
      }
      
      List.Add(
        new Vehicle(id)
        {
          Manufacturer = manufacturer,
          Model = model,
          MakeYear = makeYear,
          RegistrationNumber = registrationNumber,
          TankCapacity = tankCapacity,
          ServiceLimit = serviceLimit
        }
      );
    }

    /// <summary>
    /// Add a vehicle into the collection with only using a Vehicle object
    /// </summary>
    /// <param name="vehicle">The vehicle object to add into the list.</param>
    public void Add( Vehicle vehicle )
    {
      if ( vehicle.ServiceLimit == 0 )
      {
        vehicle.ServiceLimit = DefaultVehicleServiceLimit;
      }

      List.Add( vehicle );
    }

    /// <summary>
    /// Edit an existing vehicle
    /// </summary>
    /// <param name="vehicle">Vehicle to be referenced and modified</param>
    /// <param name="manufacturer">Manufacturer</param>
    /// <param name="model">Model</param>
    /// <param name="makeYear">Make Year</param>
    /// <param name="registrationNumber">Registration Number</param>
    /// <param name="tankCapacity">Tank Capacity. Up to 2 decimal points.</param>
    /// <param name="serviceLimit">Service Limit</param>
    public void Edit( Vehicle vehicle, string manufacturer, string model, int makeYear, string registrationNumber, double tankCapacity, int serviceLimit)
    {
      var Found = List.FirstOrDefault(i => i.Id == vehicle.Id);

      if ( Found != null )
      {
        vehicle.Manufacturer = manufacturer;
        vehicle.Model = model;
        vehicle.MakeYear = makeYear;
        vehicle.RegistrationNumber = registrationNumber;
        vehicle.TankCapacity = tankCapacity;
        vehicle.ServiceLimit = serviceLimit;
      }
    }

    /// <summary>
    /// Edit an existing vehicle.
    /// </summary>
    /// <param name="vehicle">Vehicle to be referenced and edited</param>
    public void Edit( Vehicle vehicle )
    {
      var Found = List.FirstOrDefault(i => i.Id == vehicle.Id); // Find the vehicle in the list using the vehicle ID provided.
      int index = List.IndexOf( Found ); // Retrieve index of the found vehicle in the list

      List[index] = vehicle; // Replace vehicle in the list
    }

    /// <summary>
    /// Delete a vehicle
    /// </summary>
    /// <param name="vehicle">Vehicle to be deleted</param>
    public void Delete( Vehicle vehicle )
    {
      List.Remove( vehicle );
    }

    /// <summary>
    /// Retrieves the total distance of a vehicle. Requires a list of journeys.
    /// </summary>
    /// <param name="journeys">List of journeys to search.</param>
    /// <returns>The total distance travelled by the vehicle</returns>
    public int TotalDistance( ObservableCollection<Journey> journeys )
    {
      int distanceTravelledByJourneys = 0;

      foreach (Journey journey in journeys)
      {
        distanceTravelledByJourneys += journey.Distance;
      }

      return distanceTravelledByJourneys;
    }

    /// <summary>
    /// Retrieves total fuel purchased in litres. Needs a list of fuelPurchases to come up with a calculation
    /// </summary>
    /// <param name="fuelPurchases"></param>
    /// <returns></returns>
    public double TotalFuelLitres( ObservableCollection<FuelPurchase> fuelPurchases )
    {
      double litres = 0;

      // Iterates through fuel purchases and sums up litres.
      foreach ( FuelPurchase fuelPurchase in fuelPurchases )
      {
        litres += fuelPurchase.Litres;
      }

      return litres;
    }

    /// <summary>
    /// Determine gap between recent service and total travelled distance. Will require a list of journeys and services
    /// </summary>
    /// <param name="services">List of services to be checked for recent service. List must be sorted already by descending date</param>
    /// <param name="journeys">List of journeys where distance travelled to be tallied.</param>
    /// <returns></returns>
    public int TravelServiceGap( ObservableCollection<Service> services, ObservableCollection<Journey> journeys )
    {
      int gap;
      int totalDistanceTravelled = TotalDistance( journeys ); // Retrieve total distance travelled by vehicle.
      Service recentService;

      // If vehicle has a service, get the most recent service and match against odometer and total distance.
      if ( services.Count > 0 )
      {
        recentService = services[0];
        gap = ( recentService.Odometer + DefaultVehicleServiceLimit ) - totalDistanceTravelled;
      }
      else
      {
        // Vehicle has no service and is safe to assume it travelled that far with no service.
        gap = totalDistanceTravelled;
      }

      return gap;
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
      ObservableCollection<Vehicle> listSortedID = new ObservableCollection<Vehicle>(
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