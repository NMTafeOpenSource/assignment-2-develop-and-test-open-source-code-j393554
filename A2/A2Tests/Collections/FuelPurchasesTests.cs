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
  public class FuelPurchasesTests : MockObjects
  {
    [TestMethod()]
    public void AddTest()
    {
      FuelPurchases2.Add( Vehicles3.List[0], 357.11m, 39.23, new DateTime(2019, 11, 18, 22, 15, 00));

      Assert.AreEqual(357.11m, FuelPurchases2.List[0].Cost);
    }

    [TestMethod()]
    public void EditTest()
    {
      FuelPurchases1.Edit( FuelPurchases1.List[0], FuelPurchases1.List[0].Cost, 13.37, FuelPurchases1.List[0].Date);
      Assert.AreEqual(13.37, FuelPurchases1.List[0].Litres);
    }

    [TestMethod()]
    public void DeleteTest()
    {
      FuelPurchases1.Delete(FuelPurchases1.List[0]);
      Assert.AreEqual(1, FuelPurchases1.List.Count);
    }

    [TestMethod()]
    public void RecentTest()
    {
      Assert.AreEqual( 49.57, (FuelPurchases1.Recent(Vehicles2.List[0]).Litres));
    }

    [TestMethod()]
    public void ByVehicleTest()
    {
      Assert.AreEqual(2, (FuelPurchases1.ByVehicle(Vehicles2.List[0])).Count);
    }

    [TestMethod()]
    public void FindIdTest()
    {
      Assert.AreEqual(3, FuelPurchases1.FindId());
    }
  }
}