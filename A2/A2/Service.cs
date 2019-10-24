using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  class Service
  {
    public const int SERVICE_KILOMETER_LIMIT = 10000;

    private int lastServiceOdometerKm = 0;
    private int serviceCount = 0;
    private Nullable<DateTime> lastServiceDate = null;


    public int getLastServiceOdometerKm()
    {
      return this.lastServiceOdometerKm;
    }

    public void recordService( int distance, DateTime serviceDate )
    {
      this.lastServiceOdometerKm = distance;
      this.lastServiceDate = serviceDate;
      this.serviceCount++;
    }

    public int getServiceCount()
    {
      return this.serviceCount;
    }

    public int getTotalScheduledServices()
    {
      return (int) this.lastServiceOdometerKm / SERVICE_KILOMETER_LIMIT;
    }
  }
}
