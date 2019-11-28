using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Vehicle
  {
    public int Id { get; private set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public int MakeYear { get; set; }
    public string RegistrationNumber { get; set; }
    public double TankCapacity { get; set; }
    public int ServiceLimit { get; set; }
    public string Name
    {
      get
      {
        string name = "";

        if ( Manufacturer != "" )
        {
          name += Manufacturer;

          if ( Model != "" )
          {
            name += " " + Model;
            name += " (" + MakeYear.ToString() + ")";
          }
        }

        return name;
      }
    }

    /// <summary>
    /// Vehicle class. What else is there to say?
    /// </summary>
    /// <param name="id">Id to be used for this vehicle.</param>
    public Vehicle( int id )
    {
      Id = id;
    }
  }
}
