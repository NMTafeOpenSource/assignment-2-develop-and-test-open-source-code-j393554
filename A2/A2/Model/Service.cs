using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Service
  {
    public const int SERVICE_KILOMETER_LIMIT = 10000;

    private int lastServiceOdometerKm = 0;
    private int serviceCount = 0;
    private Nullable<DateTime> lastServiceDate = null;


    public int GetLastServiceOdometerKm()
    {
      return lastServiceOdometerKm;
    }

    public void RecordService( int distance, DateTime serviceDate )
    {
      lastServiceOdometerKm = distance;
      lastServiceDate = serviceDate;
      serviceCount++;
    }

    public int GetServiceCount()
    {
      return serviceCount;
    }

    public int GetTotalScheduledServices()
    {
      return lastServiceOdometerKm / SERVICE_KILOMETER_LIMIT;
    }
  }
}
