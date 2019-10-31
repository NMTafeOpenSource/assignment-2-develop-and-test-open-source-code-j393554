using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2.Tests
{
  public class JourneyMock
  {
    /// <summary>
    /// No properties set or methods invoked
    /// </summary>
    public Journey JourneyMock1;

    /// <summary>
    /// Property set and methods invoked
    /// </summary>
    public Journey JourneyMock2;

    public JourneyMock()
    {
      JourneyMock1 = new Journey();
      JourneyMock2 = new Journey( 78517 );
    }
  }
}
