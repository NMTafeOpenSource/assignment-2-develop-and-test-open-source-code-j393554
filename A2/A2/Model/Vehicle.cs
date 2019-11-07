using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Vehicle
  {
    public string Werg = "werg";
    public string Manufacturer { get; private set; }
    public string Model { get; private set; }
    public int MakeYear { get; private set; }
    public string RegistrationNumber { get; private set; }
    public double TankCapacity { get; private set; }
    public int Odometer { get; private set; }
    private readonly FuelPurchase fuelPurchase;

    public Vehicle(
      string manufacturer,
      string model,
      int makeYear,
      string registrationNumber,
      double tankCapacity,
      int odometer = 0
    )
    {
      Manufacturer = manufacturer;
      Model = model;
      MakeYear = makeYear;
      RegistrationNumber = registrationNumber;
      TankCapacity = tankCapacity;
      Odometer = odometer;
      fuelPurchase = new FuelPurchase();
    }

    public void AddDistance( int kilometers )
    {
      Odometer += kilometers;
    }

    public void SetOdometer( int kilometers )
    {
      Odometer = kilometers;
    }

    public void AddFuel(double litres, double price)
    {
      fuelPurchase.PurchaseFuel(litres, price);
    }

    public double GetFuelLitres()
    {
      return fuelPurchase.GetFuel();
    }

    public double GetFuelCost()
    {
      return fuelPurchase.GetCost();
    }
  }
}
