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
  public class VehicleTests : MockObjects
  {
    [TestMethod()]
    public void NameTest()
    {
      Assert.AreEqual( "BMW X5 (2006)", Vehicles2.List[0].Name );
    }
  }
}