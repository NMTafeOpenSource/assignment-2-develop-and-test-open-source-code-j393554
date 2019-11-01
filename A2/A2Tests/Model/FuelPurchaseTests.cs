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
  public class FuelPurchaseTests : FuelPurchaseMock
  {
    [TestMethod()]
    public void GetFuelEconomyTest()
    {
      Assert.AreEqual( 61, FuelPurchaseMock2.GetFuelEconomy() );
    }

    [TestMethod()]
    public void SetFuelEconomyTest()
    {
      FuelPurchaseMock1.SetFuelEconomy( 13 );
      Assert.AreEqual( 13, FuelPurchaseMock1.GetFuelEconomy() );
    }

    [TestMethod()]
    public void GetFuelTest()
    {
      Assert.AreEqual( 47.29, FuelPurchaseMock2.GetFuel() );
    }

    [TestMethod()]
    public void GetCostTest()
    {
      Assert.AreEqual( 19.41, FuelPurchaseMock2.GetCost() );
    }

    [TestMethod()]
    public void PurchaseFuelTest()
    {
      FuelPurchaseMock1.PurchaseFuel( 17.2, 89.61 );
      Assert.AreEqual( 17.2, FuelPurchaseMock1.GetFuel() );
      Assert.AreEqual( 89.61, FuelPurchaseMock1.GetCost() );
    }
  }
}