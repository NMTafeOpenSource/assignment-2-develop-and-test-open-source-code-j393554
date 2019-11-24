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
  public class JourneyTests : MockObjects
  {
    [TestMethod()]
    public void VehicleIdTest()
    {
      Assert.AreEqual(1, Journeys1.List[0].VehicleId);
    }

    [TestMethod()]
    public void JourneyExternalDateTest()
    {
      Assert.AreEqual("2019-01-15 03:45 PM", Journeys1.List[0].ExternalDate);
    }
  }
}