using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class FuelPurchase
  {
    public int Id { get; private set; }
    public int VehicleId
    {
      get
      {
        return vehicle.Id;
      }
    }
    private readonly Vehicle vehicle;
    public double Litres { get; set; }
    public decimal Cost { get; set; }
    public DateTime Date { get; set; }

    /// <summary>
    /// Fuel Purchase class for a vehicle. Requires an Id and a Vehicle class as arguments.
    /// </summary>
    /// <param name="id">Id to use for this Fuel Purchase</param>
    /// <param name="vehicle">Vehicle to be used on this Fuel Purchase and to be displayed in VehicleId</param>
    public FuelPurchase( int id, Vehicle vehicle )
    {
      this.vehicle = vehicle;
      Id = id;
    }
  }
}
