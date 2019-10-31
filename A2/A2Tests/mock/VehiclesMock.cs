using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2.Tests
{
  public class VehiclesMock
  {
    // Vehicle with no Odometer Set
    public Vehicle VehicleMock1;

    // Vehicle with an Odometer Set
    public Vehicle VehicleMock2;

    public VehiclesMock()
    {
      VehicleMock1 = new Vehicle(
        "Holden",
        "Commodore Sportwagon RS-V",
        2019,
        "1DWU656",
        40
      );

      VehicleMock2 = new Vehicle(
        "Ford",
        "Ranger XL",
        2015,
        "1GVL526",
        80,
        46399
      );
    }
  }
}
