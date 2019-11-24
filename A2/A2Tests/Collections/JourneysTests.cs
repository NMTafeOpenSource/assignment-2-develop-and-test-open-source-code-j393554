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
  public class JourneysTests : MockObjects
  {
    [TestMethod()]
    public void AddTest()
    {
      Journeys2.Add(Vehicles3.List[0], new DateTime(2019, 1, 15, 15, 45, 00), 10);
      Assert.AreEqual(1, Journeys2.List.Count);
    }

    [TestMethod()]
    public void EditTest()
    {
      DateTime TestDateTime = new DateTime(2019, 1, 15, 15, 45, 00);

      Journeys1.Edit(Journeys1.List[0], TestDateTime, 33);
      Assert.AreEqual(33, Journeys1.List[0].Distance);
    }

    [TestMethod()]
    public void DeleteTest()
    {
      Journeys1.Delete(Journeys1.List[0]);
      Assert.AreEqual(1, Journeys1.List.Count);
    }

    [TestMethod()]
    public void RecentTest()
    {
      Assert.AreEqual(20, (Journeys1.Recent(Vehicles2.List[0])).Distance);
    }

    [TestMethod()]
    public void ByVehicleTest()
    {
      Assert.AreEqual(2, (Journeys1.ByVehicle(Vehicles2.List[0])).Count);
    }

    [TestMethod()]
    public void FindIdTest()
    {
      Assert.AreEqual(3, Journeys1.FindId());
    }
  }
}