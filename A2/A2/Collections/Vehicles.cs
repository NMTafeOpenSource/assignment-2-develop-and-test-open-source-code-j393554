using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace A2
{
  public class Vehicles
  {
    public ObservableCollection<Vehicle> List { get; private set; }
    public int DefaultVehicleServiceLimit = 10000;
    private Database database;
    private Offline offline;
    private readonly string fileName = "vehicles.json";

    /// <summary>
    /// Collection class designed to coordinate and handle operations on a list of vehicles.
    /// </summary>
    public Vehicles()
    {
      List = new ObservableCollection<Vehicle>();
    }

    /// <summary>
    /// Load vehicles from the appropriate data sources
    /// </summary>
    /// <param name="offline">Offline mode class capabilities</param>
    /// <param name="database">The database resource to be used to query. If not set, class will go offline and use local datasource</param>
    public void Load( Offline offline, Database database = null )
    {
      // Check if we can use a database as a resource. If not, go offline/
      if ( database != null )
      {
        this.database = database;
        this.offline = offline;

        try
        {
          // Prepare statement for query
          string statement = "SELECT * FROM vehicles";

          // Execute query
          using (MySqlCommand sqlCommand = new MySqlCommand(statement, database.Connection))
          {
            DataTable DataTable = new DataTable();
            MySqlDataAdapter DataAdapter = new MySqlDataAdapter(sqlCommand);
            DataAdapter.Fill(DataTable);

            // Map result of query into collection
            List = MapDataTableToCollection(DataTable);

            // Save mapped results offline
            offline.Load(fileName);
            SaveOffline();
          }
        }
        catch (Exception exception)
        {
          MessageBox.Show(exception.Message);
        }
      }
      else
      {
        this.offline = offline;

        // Load last queried data from saved JSON and map to collection
        offline.Load(fileName);
        List = MapJsonToCollection(offline.List);
      }
    }

    /// <summary>
    /// Saves the information on this collection to JSON
    /// </summary>
    /// <returns>True if it was saved successfully. Otherwise false.</returns>
    public bool SaveOffline()
    {
      // Watch out for empty lists that inadvertently wipe local data source.
      if (List.Count > 0)
      {
        try
        {
          offline.Save(List.ToList());
          return true;
        }
        catch (Exception)
        {
          return false;
        }
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Maps a deserialized JSON to the class used in the collection.
    /// </summary>
    /// <param name="list">The list of deserialized JSON</param>
    /// <returns>An observable collection object containing the collection of items on this collection</returns>
    public ObservableCollection<Vehicle> MapJsonToCollection( List<dynamic> list )
    {
      ObservableCollection<Vehicle> newList = new ObservableCollection<Vehicle>();

      foreach ( dynamic item in list )
      {
        newList.Add(
          new Vehicle(int.Parse(item.Id.ToString()))
          {
            Manufacturer = item.Manufacturer.ToString(),
            Model = item.Model.ToString(),
            MakeYear = int.Parse(item.MakeYear.ToString()),
            RegistrationNumber = item.RegistrationNumber.ToString(),
            TankCapacity = double.Parse(item.TankCapacity.ToString()),
            ServiceLimit = int.Parse(item.ServiceLimit.ToString())
          }
        );
      }

      return newList;
    }

    /// <summary>
    /// Maps a DataTable to the class used in the collection
    /// </summary>
    /// <param name="dataTable">The DataTable containing the information to be match into the class used in the collection</param>
    /// <returns>An observable collection object containing the collection of items on this collection</returns>
    public ObservableCollection<Vehicle> MapDataTableToCollection( DataTable dataTable )
    {
      ObservableCollection<Vehicle> list = new ObservableCollection<Vehicle>();

      foreach (DataRow row in dataTable.Rows)
      {
        list.Add(
          new Vehicle(int.Parse(row["id"].ToString()))
          {
            Manufacturer = row["manufacturer"].ToString(),
            Model = row["model"].ToString(),
            MakeYear = int.Parse(row["year"].ToString()),
            RegistrationNumber = row["registration"].ToString(),
            TankCapacity = double.Parse(row["tank_capacity"].ToString()),
            ServiceLimit = int.Parse(row["service_limit"].ToString())
          }
        );
      }

      return list;
    }

    /// <summary>
    /// Add a vehicle into the collection
    /// </summary>
    /// <param name="manufacturer">Manufacturer</param>
    /// <param name="model">Model</param>
    /// <param name="makeYear">Make Year</param>
    /// <param name="registrationNumber">Registration Number</param>
    /// <param name="tankCapacity">Tank Capacity. Up to 2 decimal points.</param>
    /// <param name="serviceLimit">Service Limit</param>
    /// <param name="id">(Optional) ID of the vehicle</param>
    public void Add(string manufacturer, string model, int makeYear, string registrationNumber, double tankCapacity, int serviceLimit = 0, int id = 0)
    {
      if ( id == 0 )
      {
        id = FindId();
      }

      if ( serviceLimit == 0 )
      {
        serviceLimit = DefaultVehicleServiceLimit;
      }

      Vehicle vehicle = new Vehicle(id)
      {
        Manufacturer = manufacturer,
        Model = model,
        MakeYear = makeYear,
        RegistrationNumber = registrationNumber,
        TankCapacity = tankCapacity,
        ServiceLimit = serviceLimit
      };

      List.Add( vehicle );

      string statement =
        $"INSERT INTO `vehicles` ( `id`, `manufacturer`, `model`, `year`, `registration`, `tank_capacity`, `service_limit`) " +
        $"VALUES (" +
          $"{vehicle.Id}," +
          $"'{vehicle.Manufacturer}'," +
          $"'{vehicle.Model.ToString()}'," +
          $"{vehicle.MakeYear.ToString()}," +
          $"'{vehicle.RegistrationNumber}'," +
          $"{vehicle.TankCapacity}," +
          $"{vehicle.ServiceLimit.ToString()}" +
        $")";

      // Execute statement if able
      ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Add a vehicle into the collection with only using a Vehicle object
    /// </summary>
    /// <param name="vehicle">The vehicle object to add into the list.</param>
    public void Add( Vehicle vehicle )
    {
      if ( vehicle.ServiceLimit == 0 )
      {
        vehicle.ServiceLimit = DefaultVehicleServiceLimit;
      }

      List.Add( vehicle );

      // Prepare SQL query
      string statement =
        $"INSERT INTO `vehicles` ( `id`, `manufacturer`, `model`, `year`, `registration`, `tank_capacity`, `service_limit`) " +
        $"VALUES (" +
          $"{vehicle.Id}," +
          $"'{vehicle.Manufacturer}'," +
          $"'{vehicle.Model.ToString()}'," +
          $"{vehicle.MakeYear.ToString()}," +
          $"'{vehicle.RegistrationNumber}'," +
          $"{vehicle.TankCapacity}," +
          $"{vehicle.ServiceLimit.ToString()}" +
        $")";

      // Execute statement if able
      ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Edit an existing vehicle
    /// </summary>
    /// <param name="vehicle">Vehicle to be referenced and modified</param>
    /// <param name="manufacturer">Manufacturer</param>
    /// <param name="model">Model</param>
    /// <param name="makeYear">Make Year</param>
    /// <param name="registrationNumber">Registration Number</param>
    /// <param name="tankCapacity">Tank Capacity. Up to 2 decimal points.</param>
    /// <param name="serviceLimit">Service Limit</param>
    public void Edit( Vehicle vehicle, string manufacturer, string model, int makeYear, string registrationNumber, double tankCapacity, int serviceLimit)
    {
      var Found = List.FirstOrDefault(i => i.Id == vehicle.Id);

      if ( Found != null )
      {
        vehicle.Manufacturer = manufacturer;
        vehicle.Model = model;
        vehicle.MakeYear = makeYear;
        vehicle.RegistrationNumber = registrationNumber;
        vehicle.TankCapacity = tankCapacity;
        vehicle.ServiceLimit = serviceLimit;

        string statement =
          $"UPDATE `vehicles`" +
          $"SET " +
            $"`manufacturer` = '{vehicle.Manufacturer}', " +
            $"`model` = '{vehicle.Model}', " +
            $"`year` = {vehicle.MakeYear.ToString()}, " +
            $"`registration` = '{vehicle.RegistrationNumber}', " +
            $"`tank_capacity` = {vehicle.TankCapacity.ToString()}, " +
            $"`service_limit` = {vehicle.ServiceLimit.ToString()} " +
          $"WHERE `id` = {vehicle.Id.ToString()};";

        // Execute statement if able
        ExecuteNonQuery(statement);
      }
    }

    /// <summary>
    /// Edit an existing vehicle.
    /// </summary>
    /// <param name="vehicle">Vehicle to be referenced and edited</param>
    public void Edit( Vehicle vehicle )
    {
      var Found = List.FirstOrDefault(i => i.Id == vehicle.Id); // Find the vehicle in the list using the vehicle ID provided.
      int index = List.IndexOf( Found ); // Retrieve index of the found vehicle in the list

      List[index] = vehicle; // Replace vehicle in the list

      string statement =
        $"UPDATE `vehicles`" +
        $"SET " +
          $"`manufacturer` = '{vehicle.Manufacturer}', " +
          $"`model` = '{vehicle.Model}', " +
          $"`year` = {vehicle.MakeYear.ToString()}, " +
          $"`registration` = '{vehicle.RegistrationNumber}', " +
          $"`tank_capacity` = {vehicle.TankCapacity.ToString()}, " +
          $"`service_limit` = {vehicle.ServiceLimit.ToString()} " +
        $"WHERE `id` = {vehicle.Id.ToString()};";

      // Execute statement if able
      ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Delete a vehicle
    /// </summary>
    /// <param name="vehicle">Vehicle to be deleted</param>
    public void Delete( Vehicle vehicle )
    {
      List.Remove( vehicle );

      string statement =
        $"DELETE FROM `vehicles`" +
        $"WHERE `id` = {vehicle.Id.ToString()};";

      // Execute statement if able
      ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Retrieves the total distance of a vehicle. Requires a list of journeys.
    /// </summary>
    /// <param name="journeys">List of journeys to search.</param>
    /// <returns>The total distance travelled by the vehicle</returns>
    public int TotalDistance( ObservableCollection<Journey> journeys )
    {
      int distanceTravelledByJourneys = 0;

      foreach (Journey journey in journeys)
      {
        distanceTravelledByJourneys += journey.Distance;
      }

      return distanceTravelledByJourneys;
    }

    /// <summary>
    /// Retrieves total fuel purchased in litres. Needs a list of fuelPurchases to come up with a calculation
    /// </summary>
    /// <param name="fuelPurchases"></param>
    /// <returns></returns>
    public double TotalFuelLitres( ObservableCollection<FuelPurchase> fuelPurchases )
    {
      double litres = 0;

      // Iterates through fuel purchases and sums up litres.
      foreach ( FuelPurchase fuelPurchase in fuelPurchases )
      {
        litres += fuelPurchase.Litres;
      }

      return litres;
    }

    /// <summary>
    /// Determine gap between recent service and total travelled distance. Will require a list of journeys and services
    /// </summary>
    /// <param name="services">List of services to be checked for recent service. List must be sorted already by descending date</param>
    /// <param name="journeys">List of journeys where distance travelled to be tallied.</param>
    /// <returns></returns>
    public int TravelServiceGap( ObservableCollection<Service> services, ObservableCollection<Journey> journeys )
    {
      int gap;
      int totalDistanceTravelled = TotalDistance( journeys ); // Retrieve total distance travelled by vehicle.
      Service recentService;

      // If vehicle has a service, get the most recent service and match against odometer and total distance.
      if ( services.Count > 0 )
      {
        recentService = services[0];
        gap = ( recentService.Odometer + DefaultVehicleServiceLimit ) - totalDistanceTravelled;
      }
      else
      {
        // Vehicle has no service and is safe to assume it travelled that far with no service.
        gap = totalDistanceTravelled;
      }

      return gap;
    }

    /// <summary>
    /// Executes an SQL statement where a response is not expected.
    /// </summary>
    /// <param name="statement">The statement to execute</param>
    /// <returns>True if statement was successfully run, false otherwise.</returns>
    private bool ExecuteNonQuery( string statement )
    {
      if (database == null)
      {
        return false;
      }

      try
      {
        database.Connection.Open();

        using (MySqlCommand query = new MySqlCommand(statement, database.Connection))
        {
          query.ExecuteNonQuery();
        }

        return true;
      }
      catch (MySqlException Ex)
      {
        return false;
      }
      finally
      {
        database.Connection.Close();
      }
    }

    /// <summary>
    /// Finds the most recent and suitable ID to be used for a new item.
    /// </summary>
    /// <returns>Most suitable ID to be used next for a new item.</returns>
    public int FindId()
    {
      int id;
      int tail;

      //if ( List.Count > 0 ) {
        // Sort list by ID
        ObservableCollection<Vehicle> listSortedID = new ObservableCollection<Vehicle>(
          List.OrderBy(j => j.Id).ToList()
        );
      //}


      tail = listSortedID.Count;

      if (tail == 0)
      {
        id = 1;
      }
      else
      {
        tail--;
        id = listSortedID[tail].Id + 1;
      }

      return id;
    }
  }
}