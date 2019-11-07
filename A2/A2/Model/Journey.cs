using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Journey
  {
    private int kilometers;

    public Journey( int kilometers = 0 )
    {
      this.kilometers = kilometers;
    }

    public void AddKilometers( int kilometers )
    {
      this.kilometers += kilometers;
    }

    public int GetKilometers()
    {
      return kilometers;
    }
  }
}
