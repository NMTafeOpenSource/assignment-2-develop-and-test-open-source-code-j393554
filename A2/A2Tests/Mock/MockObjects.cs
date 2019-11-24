using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2.Tests
{
  public class MockObjects
  {
    public Vehicles Vehicles1; // Empty vehicles
    public Vehicles Vehicles2; // With vehicles and attachable journeys, fuel purchases and services
    public Vehicles Vehicles3; // Vehicles only

    public Journeys Journeys1; // Attachable to Vehicles2
    public Journeys Journeys2 = new Journeys();

    public Services Services1;
    public Services Services2 = new Services();

    public Vehicle Vehicle1;

    public FuelPurchases FuelPurchases1;
    public FuelPurchases FuelPurchases2 = new FuelPurchases();

    public MockObjects()
    {
      Vehicles1 = new Vehicles();
      Vehicles2 = new Vehicles();
      Vehicles3 = new Vehicles();

      Journeys1 = new Journeys();

      Services1 = new Services();

      FuelPurchases1 = new FuelPurchases();

      Vehicles2.Add("BMW", "X5", 2006, "1BGZ784", 93);
      Vehicles2.Add("Tesla", "Roadster", 2008, "8HDZ576", 0);

      Vehicles3.Add("BMW", "X5", 2006, "1BGZ784", 93);
      Vehicles3.Add("Tesla", "Roadster", 2008, "8HDZ576", 0);

      Journeys1.Add(Vehicles2.List[0], new DateTime(2019, 1, 15, 15, 45, 00), 10);
      Journeys1.Add(Vehicles2.List[0], new DateTime(2019, 3, 16, 21, 10, 00), 20);

      FuelPurchases1.Add(Vehicles2.List[0], 357.11m, 39.23, new DateTime(2019, 11, 18, 22, 15, 00));
      FuelPurchases1.Add(Vehicles2.List[0], 161.80m, 49.57, new DateTime(2019, 11, 21, 12, 15, 00));

      Services1.Add(Vehicles2.List[0], 10, 473.02m, new DateTime(2018, 12, 20));
      Services1.Add(Vehicles2.List[0], 15, 537.20m, new DateTime(2019, 3, 16));

      Vehicle1 = new Vehicle(17)
      {
        Manufacturer = "Chevrolet",
        Model = "Cadillac",
        MakeYear = 1959,
        RegistrationNumber = "C4D1LL4C",
        TankCapacity = 79
      };
    }
  }
}
