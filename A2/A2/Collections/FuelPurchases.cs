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

    public void Load()
    {
      // TODO: Load Journeys with SQL Statement
    }

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

    public void Delete( FuelPurchase fuelPurchase )
    {
      List.Remove(fuelPurchase);
    }

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