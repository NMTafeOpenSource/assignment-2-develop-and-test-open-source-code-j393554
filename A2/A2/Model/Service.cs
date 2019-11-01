using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Service
  {
    public int Id { get; private set; }
    public int VehicleId {
      get {
        return vehicle.Id;
      }
    }

    private readonly Vehicle vehicle;

    public int Odometer { get; set; }
    public decimal Cost { get; set; }
    public DateTime Date { get; set; }

    public Service ( int id, Vehicle vehicle )
    {
      this.vehicle = vehicle;
      Id = id;
    }
  }
}
