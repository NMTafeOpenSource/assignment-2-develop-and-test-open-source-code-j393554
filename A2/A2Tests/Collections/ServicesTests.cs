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
  public class ServicesTests : MockObjects
  {
    [TestMethod()]
    public void AddTest()
    {
      Services2.Add( Vehicles2.List[0], 10, 473.02m, new DateTime(2018, 12, 20));

      Assert.AreEqual( 1, Services2.List.Count );
    }

    [TestMethod()]
    public void EditTest()
    {
      Services1.Edit( Services1.List[0], 300, 500m, new DateTime(2018, 12, 20));

      Assert.AreEqual( 300, Services1.List[0].Odometer );
    }

    [TestMethod()]
    public void DeleteTest()
    {
      Services1.Delete(Services1.List[0]);
      Assert.AreEqual(1, Services1.List.Count);
    }

    [TestMethod()]
    public void RecentTest()
    {
      Assert.AreEqual(15, (Services1.Recent(Vehicles2.List[0])).Odometer);
    }

    [TestMethod()]
    public void ByVehicleTest()
    {
      Assert.AreEqual(2, (Services1.ByVehicle(Vehicles2.List[0])).Count);
    }

    [TestMethod()]
    public void FindIdTest()
    {
      Assert.AreEqual(3, Services1.FindId());
    }
  }
}