using System;

namespace A2.Tests
{
  public class ServiceMock
  {
    /// <summary>
    /// Mock service with no properties modified or called
    /// </summary>
    public Service ServiceMock1;

    /// <summary>
    /// Mock service with properties modified and methods invoked
    /// </summary>
    public Service ServiceMock2;

    public ServiceMock()
    {
      ServiceMock1 = new Service();

      ServiceMock2 = new Service();
      ServiceMock2.RecordService( 89, new DateTime( 2018, 1, 26, 0, 0, 0 ) );
    }
  }
}
