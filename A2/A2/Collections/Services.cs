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
  public class Services
  {
    public ObservableCollection<Service> List { get; private set; }
    public ObservableCollection<Service> SelectedItem { get; set; }
    private Database database;
    private Offline offline;
    private readonly string fileName = "services.json";

    /// <summary>
    /// Collection class designed to coordinate and handle operations on a list of services.
    /// </summary>
    public Services()
    {
      List = new ObservableCollection<Service>();
    }

    /// <summary>
    /// Load services from the appropriate data sources
    /// </summary>
    /// <param name="vehicleList">List of vehicles required to match it to the services</param>
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
          string statement = "SELECT * FROM services";

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
    public ObservableCollection<Service> MapJsonToCollection( List<dynamic> list, ObservableCollection<Vehicle> vehicleList )
    {
      ObservableCollection<Service> newList = new ObservableCollection<Service>();

      foreach (dynamic item in list)
      {
        // Find vehicle as listed in VehicleId as we need a vehicle object to create an item in the collection
        var vehicle = vehicleList.FirstOrDefault(v => v.Id == int.Parse(item.VehicleId.ToString()));

        newList.Add(
          new Service(int.Parse(item.Id.ToString()), vehicle)
          {
            Odometer = int.Parse(item.Odometer.ToString()),
            Cost = decimal.Parse(item.Cost.ToString()),
            Date = DateTime.Parse(item.Date.ToString()),
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
    public ObservableCollection<Service> MapDataTableToCollection( DataTable dataTable, ObservableCollection<Vehicle> vehicleList )
    {
      ObservableCollection<Service> list = new ObservableCollection<Service>();

      foreach (DataRow row in dataTable.Rows)
      {
        // Find vehicle as listed in VehicleId as we need a vehicle object to create an item in the collection
        var vehicle = vehicleList.FirstOrDefault(v => v.Id == int.Parse(row["vehicle_id"].ToString()));

        list.Add(
          new Service( int.Parse( row["id"].ToString() ) , vehicle )
          {
            Odometer = int.Parse(row["odometer"].ToString()),
            Cost = decimal.Parse(row["cost"].ToString()),
            Date = DateTime.Parse(row["date"].ToString())
          }
        );
      }

      return list;
    }

    /// <summary>
    /// Add a service into the collection
    /// </summary>
    /// <param name="vehicle">Vehicle to be referenced on this service</param>
    /// <param name="odometer">The odometer of the vehicle when serviced</param>
    /// <param name="cost">Cost of service. Up to 2 decimal points</param>
    /// <param name="date">Date of service</param>
    public void Add( Vehicle vehicle, int odometer, decimal cost, DateTime date )
    {
      int id = FindId();
      Service service = new Service(id, vehicle)
      {
        Odometer = odometer,
        Cost = cost,
        Date = date
      };

      List.Add( service );

      // Prepare SQL query
      string statement =
        $"INSERT INTO `services` ( `id`, `vehicle_id`, `odometer`, `cost`, `date`)" +
        $"VALUES (" +
          $"{service.Id}," +
          $"{service.VehicleId}," +
          $"{service.Odometer.ToString()}," +
          $"{service.Cost.ToString()}," +
          $"'{service.Date.ToString("yyyy-MM-dd")}'" +
        $")";

      // Execute statement if able
      ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Edit an existing service
    /// </summary>
    /// <param name="service">The service to be referenced and to be modified</param>
    /// <param name="odometer">The odometer of the vehicle when serviced</param>
    /// <param name="cost">Cost of service. Up to 2 decimal points</param>
    /// <param name="date">Date of service</param>
    public void Edit( Service service, int odometer, decimal cost, DateTime date )
    {
      var FoundService = List.FirstOrDefault(i => i.Id == service.Id);

      if (FoundService != null)
      {
        FoundService.Odometer = odometer;
        FoundService.Cost = cost;
        FoundService.Date = date;

        string statement =
          $"UPDATE `services`" +
          $"SET " +
            $"`odometer` = {service.Odometer.ToString()}, " +
            $"`cost` = {service.Cost.ToString()}, " +
            $"`date` = '{service.Date.ToString("yyyy-MM-dd")}'" +
          $"WHERE `id` = {service.Id.ToString()};";

        // Execute statement if able
        ExecuteNonQuery(statement);
      }
    }

    /// <summary>
    /// Delete a service
    /// </summary>
    /// <param name="service">Service to be deleted</param>
    public void Delete( Service service )
    {
      List.Remove( service );

      string statement =
        $"DELETE FROM `services`" +
        $"WHERE `id` = {service.Id.ToString()};";

      // Execute statement if able
      ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Retrieves the most recent service
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve the most recent service</param>
    /// <returns>The most recent service</returns>
    public Service Recent( Vehicle vehicle )
    {
      Service RecentService = null;

      if ( List.Count > 0 )
      {
        // Group Journeys by Vehicle ID.
        ObservableCollection<Service> VehicleServices = ByVehicle( vehicle );
      
        if ( VehicleServices.Count > 0 )
        {
          RecentService = VehicleServices[0];
        }
      }

      return RecentService;
    }

    /// <summary>
    /// Retrieves services by a supplied vehicle.
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve all services</param>
    /// <returns>An ObservableCollection of the services by the vehicle passed</returns>
    public ObservableCollection<Service> ByVehicle( Vehicle vehicle )
    {
      ObservableCollection<Service> vehicleServices = new ObservableCollection<Service>(
        List
          .Where(i => i.VehicleId == vehicle.Id)
          .OrderBy(i => i.Odometer)
          .Reverse()
          .ToList()
      );

      return vehicleServices;
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
      ObservableCollection<Service> listSortedID = new ObservableCollection<Service>(
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
