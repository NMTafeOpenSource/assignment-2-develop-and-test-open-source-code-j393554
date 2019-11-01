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
    public double Litres { get; set; }
    public decimal Cost { get; set; }
    public DateTime Date { get; set; }
    public int VehicleId
    {
      get
      {
        return vehicle.Id;
      }
    }

    private readonly Vehicle vehicle;

    public FuelPurchase( int id, Vehicle vehicle )
    {
      this.vehicle = vehicle;
      Id = id;
    }
  }
}
