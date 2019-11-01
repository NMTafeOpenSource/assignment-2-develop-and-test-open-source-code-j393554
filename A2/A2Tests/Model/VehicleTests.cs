using Microsoft.VisualStudio.TestTools.UnitTesting;
using A2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2.Tests
{
  [TestClass()]
  public class VehicleTests : VehiclesMock
  {
    [TestMethod()]
    public void VehicleTest()
    {
      Assert.AreEqual( 0, VehicleMock1.Odometer );
      Assert.AreEqual( 46399, VehicleMock2.Odometer );
    }

    [TestMethod()]
    public void AddDistanceTest()
    {
      VehicleMock1.AddDistance( 100 );
      Assert.AreEqual( 100, VehicleMock1.Odometer );
    }

    [TestMethod()]
    public void SetOdometerTest()
    {
      VehicleMock1.SetOdometer( 100 );
      Assert.AreEqual( 100, VehicleMock1.Odometer );
    }

    [TestMethod()]
    public void AddFuelTest()
    {
      VehicleMock1.AddFuel( 2.3, 5.7 );
      VehicleMock1.AddFuel( 11.13, 17.19 );

      Assert.AreEqual( 2.3 + 11.13, VehicleMock1.GetFuelLitres() );
      Assert.AreEqual( 5.7 + 17.19, VehicleMock1.GetFuelCost() );
    }
  }
}