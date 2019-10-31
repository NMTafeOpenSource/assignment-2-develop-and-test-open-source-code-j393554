using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2.Tests
{
  public class FuelPurchaseMock
  {
    /// <summary>
    /// Vanilla, no parameters set
    /// </summary>
    public FuelPurchase FuelPurchaseMock1;

    /// <summary>
    /// With parameters set
    /// </summary>
    public FuelPurchase FuelPurchaseMock2;

    public FuelPurchaseMock()
    {
      FuelPurchaseMock1 = new FuelPurchase();
      FuelPurchaseMock2 = new FuelPurchase();

      FuelPurchaseMock2.SetFuelEconomy( 61 );
      FuelPurchaseMock2.PurchaseFuel( 47.29, 19.41 );
    }
  }
}
