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
  public class VehiclesTests : MockObjects
  {
    [TestMethod()]
    public void AddTest()
    {
      Vehicles1.Add("BMW", "X5", 2006, "1BGZ784", 93, 50);

      Assert.AreEqual( 1, Vehicles1.List.Count );
    }

    [TestMethod()]
    public void AddUsingObjectTest()
    {
      Vehicles2.Add( Vehicle1 );

      Assert.AreEqual(3, Vehicles2.List.Count);
    }

    [TestMethod()]
    public void EditTest()
    {
      Vehicles2.Add( Vehicle1 );
      string originalModel = Vehicle1.Model;

      Vehicles2.Edit( Vehicle1, Vehicle1.Manufacturer, "X" + Vehicle1.Model, Vehicle1.MakeYear, Vehicle1.RegistrationNumber, Vehicle1.TankCapacity, Vehicle1.ServiceLimit );

      Assert.AreEqual("X" + originalModel, (Vehicles2.List.Last()).Model );
    }

    [TestMethod()]
    public void EditTest1()
    {
      Vehicles2.Add(Vehicle1);

      string originalModel = Vehicle1.Model;

      Vehicle1.Model = "X" + Vehicle1.Model;

      Vehicles2.Edit(Vehicle1);

      Assert.AreEqual("X" + originalModel, (Vehicles2.List.Last()).Model);
    }

    [TestMethod()]
    public void DeleteTest()
    {
      Vehicles1.Add( Vehicle1 );
      Vehicles1.Delete( Vehicle1 );
      Assert.AreEqual( 0, Vehicles1.List.Count);
    }

    [TestMethod()]
    public void TotalDistanceTest()
    {
      int distance = Vehicles2.TotalDistance( Journeys1.ByVehicle( Vehicles2.List[0] ) );

      Assert.AreEqual( 30, distance  );
    }

    [TestMethod()]
    public void TotalFuelLitresTest()
    {
      double litres = Vehicles2.TotalFuelLitres( FuelPurchases1.ByVehicle( Vehicles2.List[0] ) );

      Assert.AreEqual(39.23 + 49.57, litres );
    }

    [TestMethod()]
    public void TravelServiceGapTest()
    {
      int gap = Vehicles2.TravelServiceGap(Services1.ByVehicle(Vehicles2.List[0]), Journeys1.ByVehicle(Vehicles2.List[0]));

      Assert.AreEqual( 9985, gap );
    }

    [TestMethod()]
    public void FindIdTest()
    {
      Vehicles1.Add( Vehicle1 );


      Assert.AreEqual( 18, Vehicles1.FindId() );
    }
  }
}