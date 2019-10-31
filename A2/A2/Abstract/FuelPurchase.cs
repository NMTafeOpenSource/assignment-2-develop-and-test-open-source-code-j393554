using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class FuelPurchase
  {
    private double fuelEconomy;
    private double litres = 0;
    private double cost = 0;

    public double GetFuelEconomy()
    {
      return fuelEconomy;
    }

    public void SetFuelEconomy(double fuelEconomy)
    {
      this.fuelEconomy = fuelEconomy;
    }

    public double GetFuel()
    {
      return litres;
    }

    public double GetCost()
    {
      return cost;
    }

    public void PurchaseFuel(double amount, double price)
    {
      litres += amount;
      cost += price;
    }
  }
}
