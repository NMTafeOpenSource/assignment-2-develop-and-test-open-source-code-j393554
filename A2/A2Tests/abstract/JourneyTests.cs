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
  public class JourneyTests : JourneyMock
  {
    [TestMethod()]
    public void JourneyTest()
    {
      Assert.AreEqual(0, JourneyMock1.GetKilometers());
      Assert.AreEqual(78517, JourneyMock2.GetKilometers());
    }

    [TestMethod()]
    public void AddKilometersTest()
    {
      JourneyMock1.AddKilometers(12739);
      Assert.AreEqual(12739, JourneyMock1.GetKilometers());
    }

    [TestMethod()]
    public void GetKilometersTest()
    {
      JourneyMock2.AddKilometers(39901);
      Assert.AreEqual(118418, JourneyMock2.GetKilometers());
    }
  }
}