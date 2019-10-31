using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  class Vehicle
  {
    public string manufacturer { get; private set; }
    public string model { get; private set; }
    public int makeYear { get; private set; }
    public string registrationNumber { get; private set; }
    public double tankCapacity { get; private set; }
    public int odometer { get; private set; }
    private FuelPurchase fuelPurchase;

    public Vehicle(string manufacturer, string model, int makeYear, string registrationNumber, double tankCapacity, int odometer = 0 )
    {
      this.manufacturer = manufacturer;
      this.model = model;
      this.makeYear = makeYear;
      this.registrationNumber = registrationNumber;
      this.tankCapacity = tankCapacity;
      this.odometer = odometer;
      this.fuelPurchase = new FuelPurchase();
    }

    public void addDistance( int kilometers )
    {
      this.odometer += kilometers;
    }

    public void setOdometer( int kilometers )
    {
      this.odometer = kilometers;
    }

    public void addFuel(double litres, double price)
    {
      this.fuelPurchase.purchaseFuel(litres, price);
    }
  }
}
