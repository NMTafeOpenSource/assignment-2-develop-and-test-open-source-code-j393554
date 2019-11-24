using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class FuelPurchases
  {
    public ObservableCollection<FuelPurchase> List { get; private set; }
    public ObservableCollection<FuelPurchase> SelectedItem { get; set; }

    public FuelPurchases()
    {
      List = new ObservableCollection<FuelPurchase>();
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
    /// Add fuel purchase into the collection
    /// </summary>
    /// <param name="vehicle">Vehicle to be referenced on this Fuel Purchase</param>
    /// <param name="cost">Cost of the fuel purchased. Up to 2 decimal points</param>
    /// <param name="litres">Fuel purchased in litres. Up to 2 decimal points</param>
    /// <param name="date">Date of purchase of fuel</param>
    public void Add( Vehicle vehicle, decimal cost, double litres, DateTime date )
    {
      int id = FindId();

      List.Add(
        new FuelPurchase( id, vehicle )
        {
          Litres = litres,
          Cost = cost,
          Date = date
        }
      );
    }

    /// <summary>
    /// Edit an existing fuel purchase
    /// </summary>
    /// <param name="fuelPurchase">The fuel purchase to be referenced and to be modified</param>
    /// <param name="cost">Cost of the fuel purchased. Up to 2 decimal points</param>
    /// <param name="litres">Fuel purchased in litres. Up to 2 decimal points</param>
    /// <param name="date">Date of purchase of fuel</param>
    public void Edit( FuelPurchase fuelPurchase, decimal cost, double litres, DateTime date )
    {
      var FoundFuelPurchase = List.FirstOrDefault(i => i.Id == fuelPurchase.Id);

      if (FoundFuelPurchase != null)
      {
        FoundFuelPurchase.Cost = cost;
        FoundFuelPurchase.Litres = litres;
        FoundFuelPurchase.Date = date;
      }
    }

    /// <summary>
    /// Delete a fuel purchase
    /// </summary>
    /// <param name="fuelPurchase">Fuel purchase to be deleted</param>
    public void Delete( FuelPurchase fuelPurchase )
    {
      List.Remove(fuelPurchase);
    }

    /// <summary>
    /// Retrieves the most recent fuel purchase
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve the most recent fuel purchase</param>
    /// <returns>The most recent fuel purchase</returns>
    public FuelPurchase Recent( Vehicle vehicle )
    {
      FuelPurchase RecentFuelPurchase = null;

      if (List.Count > 0)
      {
        // Group Journeys by Vehicle ID.
        ObservableCollection<FuelPurchase> VehicleFuelPurchases = ByVehicle(vehicle);

        if (VehicleFuelPurchases.Count > 0)
        {
          RecentFuelPurchase = VehicleFuelPurchases[0];
        }
      }

      return RecentFuelPurchase;
    }

    /// <summary>
    /// Retrieves fuel purchases by a supplied vehicle.
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve all fuel purchases</param>
    /// <returns>An ObservableCollection of the fuel purchases by the vehicle passed</returns>
    public ObservableCollection<FuelPurchase> ByVehicle( Vehicle vehicle )
    {
      ObservableCollection<FuelPurchase> VehicleFuelPurchases = new ObservableCollection<FuelPurchase>(
        List
          .Where(i => i.VehicleId == vehicle.Id)
          .OrderBy(i => i.Date)
          .Reverse()
          .ToList()
      );

      return VehicleFuelPurchases;
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
      ObservableCollection<FuelPurchase> listSortedID = new ObservableCollection<FuelPurchase>(
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