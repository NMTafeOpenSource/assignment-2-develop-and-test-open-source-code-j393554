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
  public class ServiceTests : MockObjects
  {
    [TestMethod()]
    public void VehicleIdTest()
    {
      Assert.AreEqual(1, Services1.List[0].VehicleId);
    }
  }
}