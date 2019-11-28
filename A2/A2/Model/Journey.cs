using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Journey
  {
    public int Id { get; private set; }
    private readonly Vehicle vehicle;
    public int VehicleId
    {
      get
      {
        return vehicle.Id;
      }
    }

    public DateTime Date { get; set; }
    public string ExternalDate {
      get
      {
        return Date.ToString("yyyy-MM-dd hh:mm tt");
      }
    }

    public int Distance { get; set; }

    /// <summary>
    /// Journey class for a vehicle. Requires an Id and a Vehicle class as arguments.
    /// </summary>
    /// <param name="id">Id to use for this Journey</param>
    /// <param name="vehicle">Vehicle to be used on this Journey and to be displayed in VehicleId</param>
    public Journey( int id, Vehicle vehicle )
    {
      this.vehicle = vehicle;
      Id = id;
    }
  }
}