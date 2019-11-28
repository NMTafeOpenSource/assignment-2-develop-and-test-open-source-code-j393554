using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace A2
{
  public class FuelPurchases
  {
    public ObservableCollection<FuelPurchase> List { get; private set; }
    public ObservableCollection<FuelPurchase> SelectedItem { get; set; }
    private Database database;
    private Offline offline;
    private readonly string fileName = "fuelpurchases.json";

    /// <summary>
    /// Collection class designed to coordinate and handle operations on a list of fuel purchases.
    /// </summary>
    public FuelPurchases()
    {
      List = new ObservableCollection<FuelPurchase>();
    }

    /// <summary>
    /// Load fuel purchases from the appropriate data sources
    /// </summary>
    /// <param name="vehicleList">List of vehicles required to match it to the fuel purchases</param>
    /// <param name="offline">Offline mode class capabilities</param>
    /// <param name="database">The database resource to be used to query. If not set, class will go offline and use local datasource</param>
    public void Load( ObservableCollection<Vehicle> vehicleList, Offline offline, Database database = null )
    {
      // Check if we can use a database as a resource. If not, go offline/
      if ( database != null )
      {
        this.database = database;
        this.offline = offline;

        try
        {
          // Prepare statement for query
          string statement = "SELECT * FROM fuel_purchases";

          // Execute query
          using (MySqlCommand sqlCommand = new MySqlCommand(statement, database.Connection))
          {
            DataTable DataTable = new DataTable();
            MySqlDataAdapter DataAdapter = new MySqlDataAdapter(sqlCommand);
            DataAdapter.Fill(DataTable);

            // Map result of query into collection
            List = MapDataTableToCollection(DataTable, vehicleList);

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
        List = MapJsonToCollection(offline.List, vehicleList);
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
    /// <param name="vehicleList">List of vehicles to match against the items in the list</param>
    /// <returns>An observable collection object containing the collection of items on this collection</returns>
    public ObservableCollection<FuelPurchase> MapJsonToCollection( List<dynamic> list, ObservableCollection<Vehicle> vehicleList )
    {
      ObservableCollection<FuelPurchase> newList = new ObservableCollection<FuelPurchase>();

      foreach (dynamic item in list)
      {
        // Find vehicle as listed in VehicleId as we need a vehicle object to create an item in the collection
        var vehicle = vehicleList.FirstOrDefault(v => v.Id == int.Parse(item.VehicleId.ToString()));

        newList.Add(
          new FuelPurchase(int.Parse(item.Id.ToString()), vehicle) {
            Litres = double.Parse(item.Litres.ToString()),
            Cost = decimal.Parse(item.Cost.ToString()),
            Date = DateTime.Parse(item.Date.ToString())
          }
        );
      }

      return newList;
    }

    /// <summary>
    /// Maps a DataTable to the class used in the collection
    /// </summary>
    /// <param name="dataTable">The DataTable containing the information to be match into the class used in the collection</param>
    /// <param name="vehicleList">List of vehicles to match against the vehicle as listed in the item's VehicleId</param>
    /// <returns>An observable collection object containing the collection of items on this collection</returns>
    public ObservableCollection<FuelPurchase> MapDataTableToCollection( DataTable dataTable, ObservableCollection<Vehicle> vehicleList )
    {
      ObservableCollection<FuelPurchase> list = new ObservableCollection<FuelPurchase>();

      foreach (DataRow row in dataTable.Rows)
      {
        // Find vehicle as listed in VehicleId as we need a vehicle object to create an item in the collection
        var vehicle = vehicleList.FirstOrDefault(v => v.Id == int.Parse(row["vehicle_id"].ToString()));

        list.Add(
          new FuelPurchase(int.Parse(row["id"].ToString()), vehicle) {
            Litres = double.Parse(row["litres"].ToString()),
            Cost = decimal.Parse(row["cost"].ToString()),
            Date = DateTime.Parse(row["date"].ToString())
          }
        );
      }

      return list;
    }

    /// <summary>
    /// Add fuel purchase into the collection
    /// </summary>
    /// <param name="vehicle">Vehicle to be referenced on this Fuel Purchase</param>
    /// <param name="cost">Cost of the fuel purchased. Up to 2 decimal points</param>
    /// <param name="litres">Fuel purchased in litres. Up to 2 decimal points</param>
    /// <param name="date">Date of purchase of fuel</param>
    public void Add( Vehicle vehicle, decimal cost, double litres, DateTime date )
    {
      int id = FindId();
      FuelPurchase fuelPurchase = new FuelPurchase( id, vehicle )
      {
        Litres = litres,
        Cost = cost,
        Date = date
      };

      List.Add( fuelPurchase );

      // Prepare SQL query
      string statement =
        $"INSERT INTO `fuel_purchases` ( `id`, `vehicle_id`, `litres`, `cost`, `date`)" +
        $"VALUES (" +
          $"{fuelPurchase.Id}," +
          $"{fuelPurchase.VehicleId}," +
          $"{fuelPurchase.Litres.ToString()}," +
          $"{fuelPurchase.Cost.ToString()}," +
          $"'{fuelPurchase.Date.ToString("yyyy-MM-dd")}'" +
        $")";

      // Execute statement if able
      ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Edit an existing fuel purchase
    /// </summary>
    /// <param name="fuelPurchase">The fuel purchase to be referenced and to be modified</param>
    /// <param name="cost">Cost of the fuel purchased. Up to 2 decimal points</param>
    /// <param name="litres">Fuel purchased in litres. Up to 2 decimal points</param>
    /// <param name="date">Date of purchase of fuel</param>
    public void Edit( FuelPurchase fuelPurchase, decimal cost, double litres, DateTime date )
    {
      var FoundFuelPurchase = List.FirstOrDefault(i => i.Id == fuelPurchase.Id);

      if (FoundFuelPurchase != null)
      {
        FoundFuelPurchase.Cost = cost;
        FoundFuelPurchase.Litres = litres;
        FoundFuelPurchase.Date = date;

        string statement =
          $"UPDATE `fuel_purchases`" +
          $"SET " +
            $"`litres` = {FoundFuelPurchase.Litres.ToString()}, " +
            $"`cost` = {FoundFuelPurchase.Cost.ToString()}, " +
            $"`date` = '{FoundFuelPurchase.Date.ToString("yyyy-MM-dd")}'" +
          $"WHERE `id` = {FoundFuelPurchase.Id.ToString()};";

        // Execute statement if able
        ExecuteNonQuery(statement);
      }
    }

    /// <summary>
    /// Delete a fuel purchase
    /// </summary>
    /// <param name="fuelPurchase">Fuel purchase to be deleted</param>
    public void Delete( FuelPurchase fuelPurchase )
    {
      List.Remove(fuelPurchase);

      string statement =
        $"DELETE FROM `services`" +
        $"WHERE `id` = {fuelPurchase.Id.ToString()};";

      // Execute statement if able
      ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Retrieves the most recent fuel purchase
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve the most recent fuel purchase</param>
    /// <returns>The most recent fuel purchase</returns>
    public FuelPurchase Recent( Vehicle vehicle )
    {
      FuelPurchase RecentFuelPurchase = null;

      if (List.Count > 0)
      {
        // Group Journeys by Vehicle ID.
        ObservableCollection<FuelPurchase> VehicleFuelPurchases = ByVehicle(vehicle);

        if (VehicleFuelPurchases.Count > 0)
        {
          RecentFuelPurchase = VehicleFuelPurchases[0];
        }
      }

      return RecentFuelPurchase;
    }

    /// <summary>
    /// Retrieves fuel purchases by a supplied vehicle.
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve all fuel purchases</param>
    /// <returns>An ObservableCollection of the fuel purchases by the vehicle passed</returns>
    public ObservableCollection<FuelPurchase> ByVehicle( Vehicle vehicle )
    {
      ObservableCollection<FuelPurchase> VehicleFuelPurchases = new ObservableCollection<FuelPurchase>(
        List
          .Where(i => i.VehicleId == vehicle.Id)
          .OrderBy(i => i.Date)
          .Reverse()
          .ToList()
      );

      return VehicleFuelPurchases;
    }

    /// <summary>
    /// Executes an SQL statement where a response is not expected.
    /// </summary>
    /// <param name="statement">The statement to execute</param>
    /// <returns>True if statement was successfully run, false otherwise.</returns>
    private bool ExecuteNonQuery( string statement )
    {
      if ( database == null )
      {
        return false;
      }

      try
      {
        database.Open();

        using (MySqlCommand query = new MySqlCommand(statement, database.Connection))
        {
          query.ExecuteNonQuery();
        }

        return true;
      }
      catch (MySqlException)
      {
        return false;
      }
      finally
      {
        database.Close();
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

      // Sort list by ID
      ObservableCollection<FuelPurchase> listSortedID = new ObservableCollection<FuelPurchase>(
        List.OrderBy(j => j.Id).ToList()
      );

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