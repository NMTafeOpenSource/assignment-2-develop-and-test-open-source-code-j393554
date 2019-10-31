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
  public class ServiceTests : ServiceMock
  {
    [TestMethod()]
    public void GetLastServiceOdometerKmTest()
    {
      Assert.AreEqual( 0, ServiceMock1.GetLastServiceOdometerKm() );
      Assert.AreEqual( 89, ServiceMock2.GetLastServiceOdometerKm() );
    }

    [TestMethod()]
    public void RecordServiceTest()
    {
      ServiceMock2.RecordService( 41, new DateTime( 2017, 07, 14, 1, 30, 0 ) );

      Assert.AreEqual( 2, ServiceMock2.GetServiceCount() );
      Assert.AreEqual( 41, ServiceMock2.GetLastServiceOdometerKm() );
      Assert.AreEqual( (int) 41 / 10000 , ServiceMock2.GetTotalScheduledServices());
    }

    [TestMethod()]
    public void GetServiceCountTest()
    {
      Assert.AreEqual( 0, ServiceMock1.GetServiceCount() );
      Assert.AreEqual( 1, ServiceMock2.GetServiceCount() );
    }

    [TestMethod()]
    public void GetTotalScheduledServicesTest()
    {
      Assert.AreEqual( (int) 89 / 10000, ServiceMock2.GetTotalScheduledServices());
    }
  }
}