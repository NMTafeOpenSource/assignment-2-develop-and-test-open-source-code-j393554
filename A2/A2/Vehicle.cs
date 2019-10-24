using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  class Vehicle
  {
    private string manufacturer;
    private string model;
    private int makeYear;
    // TODO add Registration Number 
    // TODO add variable for OdometerReading (in KM), 
    // TODO add variable for TankCapacity (in litres)

    private FuelPurchase fuelPurchase;

    public Vehicle(string manufacturer, string model, int makeYear)
    {
      this.manufacturer = manufacturer;
      this.model = model;
      this.makeYear = makeYear;
      this.fuelPurchase = new FuelPurchase();
    }

    // TODO Add missing getter and setter methods
    public void printDetails()
    {
      Console.WriteLine("Vehicle: " + makeYear + " " + manufacturer + " " + model);
      // TODO Display additional information about this vehicle
    }

    // TODO Create an addKilometers method which takes a parameter for distance travelled and adds it to the odometer reading. 
    // adds fuel to the car
    public void addFuel(double litres, double price)
    {
      this.fuelPurchase.purchaseFuel(litres, price);
    }
  }
}
