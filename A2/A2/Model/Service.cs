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
    private readonly Vehicle vehicle;
    public int VehicleId {
      get {
        return vehicle.Id;
      }
    }
    public int Odometer { get; set; }
    public decimal Cost { get; set; }
    public DateTime Date { get; set; }

    /// <summary>
    /// Service class for a vehicle. Requires an Id and a Vehicle class as arguments.
    /// </summary>
    /// <param name="id">Id to use for this Service</param>
    /// <param name="vehicle">Vehicle to be used on this Service and to be displayed in VehicleId</param>
    public Service ( int id, Vehicle vehicle )
    {
      this.vehicle = vehicle;
      Id = id;
    }
  }
}
