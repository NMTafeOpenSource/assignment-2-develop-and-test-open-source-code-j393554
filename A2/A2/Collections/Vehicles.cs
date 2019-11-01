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
    
    private Vehicle _SelectedVehicle;
    public Vehicle SelectedVehicle
    {
      get
      {
        return _SelectedVehicle;
      }
      set
      {
        if (_SelectedVehicle != value)
        {
          _SelectedVehicle = value;
          //RaisePropertyChanged("SelectedVehicle");
        }
      }
    }

    public Vehicles()
    {
      List = new ObservableCollection<Vehicle>();
      LoadVehicles();
    }

    public void LoadVehicles()
    {
      // TODO: Load vehicles from SQL/JSON
    }

    public void Add(string manufacturer, string model, int makeYear, string registrationNumber, double tankCapacity, int odometer, int id = 0)
    {
      if ( id == 0 )
      {
        id = FindId();
      }
      
      List.Add(
        new Vehicle(id)
        {
          Manufacturer = manufacturer,
          Model = model,
          MakeYear = makeYear,
          RegistrationNumber = registrationNumber,
          TankCapacity = tankCapacity,
          Odometer = odometer
        }
      );
    }

    public void Add( Vehicle vehicle )
    {
      List.Add( vehicle );
    }

    public void Edit( Vehicle vehicle, string manufacturer, string model, int makeYear, string registrationNumber, double tankCapacity, int odometer )
    {
      var Found = List.FirstOrDefault(i => i.Id == vehicle.Id);

      if ( Found != null )
      {
        vehicle.Manufacturer = manufacturer;
        vehicle.Model = model;
        vehicle.MakeYear = makeYear;
        vehicle.RegistrationNumber = registrationNumber;
        vehicle.TankCapacity = tankCapacity;
        vehicle.Odometer = odometer;
      }
    }

    public void Edit( Vehicle vehicle )
    {
      var Found = List.FirstOrDefault(i => i.Id == vehicle.Id);
      int index = List.IndexOf( Found );

      List[index] = vehicle;
    }

    public void Delete( Vehicle vehicle )
    {
      List.Remove( vehicle );
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

    public int TotalDistance( Vehicle vehicle, ObservableCollection<Journey> journeys )
    {
      int distanceTravelledByJourneys = vehicle.Odometer;

      if ( journeys.Count > 0 )
      {
        distanceTravelledByJourneys = journeys[0].EndOdometer;
      }

      //foreach ( Journey journey in journeys )
      //{
      //  distanceTravelledByJourneys += journey.Distance;
      //}

      //return vehicleOdometer + distanceTravelledByJourneys;

      return distanceTravelledByJourneys;
    }

    public double TotalFuelLitres( ObservableCollection<FuelPurchase> fuelPurchases )
    {
      double litres = 0;

      foreach ( FuelPurchase fuelPurchase in fuelPurchases )
      {
        litres += fuelPurchase.Litres;
      }

      return litres;
    }

    /// <summary>
    /// Determine gap between recent service and total travelled distance
    /// </summary>
    /// <param name="vehicle"></param>
    /// <param name="services"></param>
    /// <param name="journeys"></param>
    /// <returns></returns>
    public int TravelServiceGap( Vehicle vehicle, ObservableCollection<Service> services, ObservableCollection<Journey> journeys )
    {
      int gap;
      int totalDistanceTravelled = TotalDistance( vehicle, journeys );
      Service recentService;

      if ( services.Count > 0 )
      {
        recentService = services[0];
        gap = totalDistanceTravelled - recentService.Odometer;
      }
      else
      {
        gap = totalDistanceTravelled;
      }

      return gap;
    }
  }
}