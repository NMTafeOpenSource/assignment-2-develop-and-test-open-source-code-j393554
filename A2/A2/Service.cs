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
    // TODO add lastServiceDate

    public int getLastServiceOdometerKm()
    {
      return this.lastServiceOdometerKm;
    }

    public void recordService(int distance)
    {
      this.lastServiceOdometerKm = distance;
      this.serviceCount++;
    }

    public int getServiceCount()
    {
      return this.serviceCount;
    }

    public int getTotalScheduledServices()
    {
      return this.lastServiceOdometerKm / SERVICE_KILOMETER_LIMIT;
    }
  }
}
