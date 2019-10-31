using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  class Journey
  {
    private double kilometers;

    public Journey()
    {
      this.kilometers = 0;
    }

    public void addKilometers(double kilometers)
    {
      this.kilometers += kilometers;
    }

    public double getKilometers()
    {
      return this.kilometers;
    }
  }
}
