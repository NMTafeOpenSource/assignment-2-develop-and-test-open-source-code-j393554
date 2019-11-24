using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Journeys
  {
    public ObservableCollection<Journey> List { get; private set; }
    public ObservableCollection<Journey> SelectedVehicleJourneys { get; set; }

    public Journeys()
    {
      List = new ObservableCollection<Journey>();
      Load();
    }

    /// <summary>
    /// Load journeys from database;
    /// </summary>
    public void Load()
    {
      // TODO: Load Journeys with SQL Statement
    }

    /// <summary>
    /// Add new journey into the collection
    /// </summary>
    /// <param name="vehicle">Vehicle to be referenced on this Journey</param>
    /// <param name="startDate">Start date/time of journey</param>
    /// <param name="endDate">End date/time of journey</param>
    /// <param name="startOdometer">Start odometer of the journey</param>
    /// <param name="endOdometer">End odometer of the journey</param>
    public void Add( Vehicle vehicle, DateTime startDate, DateTime endDate, int startOdometer, int endOdometer )
    {
      int id = FindId();

      List.Add(
        new Journey( id, vehicle )
        {
          StartDate = startDate,
          EndDate = endDate,
          StartOdometer = startOdometer,
          EndOdometer = endOdometer,
        }
      );
    }

    /// <summary>
    /// Edit a journey using the provided journey and property values
    /// </summary>
    /// <param name="journey">Journey to be referenced and to be modified</param>
    /// <param name="startDate">Start date/time of journey</param>
    /// <param name="endDate">End date/time of journey</param>
    /// <param name="startOdometer">Start odometer of the journey</param>
    /// <param name="endOdometer">End odometer of the journey</param>
    public void EditJourney( Journey journey, DateTime startDate, DateTime endDate, int startOdometer, int endOdometer)
    {
      var FindJourney = List.FirstOrDefault( j => j.Id == journey.Id );

      if ( FindJourney != null )
      {
        FindJourney.StartDate = startDate;
        FindJourney.EndDate = endDate;
        FindJourney.StartOdometer = startOdometer;
        FindJourney.EndOdometer = endOdometer;
      }
    }

    /// <summary>
    /// Deletes a Journey
    /// </summary>
    /// <param name="journey">Journey to be deleted</param>
    public void DeleteJourney( Journey journey )
    {
      List.Remove( journey );
    }

    /// <summary>
    /// Retrieves the most recent journey
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve the most recent journey</param>
    /// <returns>The most recent journey</returns>
    public Journey recentJourney( Vehicle vehicle )
    {
      Journey recentJourney = null;

      if ( List.Count > 0 )
      {
        // Group Services by Vehicle ID.
        ObservableCollection<Journey> vehicleJourneys = ByVehicle( vehicle );
      
        if ( vehicleJourneys.Count > 0 )
        {
          recentJourney = vehicleJourneys[0];
        }
      }

      return recentJourney;
    }

    /// <summary>
    /// Retrieves journeys by a supplied vehicle.
    /// </summary>
    /// <param name="vehicle">The vehicle to retrieve all journeys</param>
    /// <returns>An ObservableCollection of the journeys by the vehicle passed</returns>
    public ObservableCollection<Journey> ByVehicle( Vehicle vehicle )
    {
      ObservableCollection<Journey> vehicleJourneys = new ObservableCollection<Journey>(
        List
          .Where(j => j.VehicleId == vehicle.Id)
          .OrderBy(j => j.EndDate)
          .Reverse()
          .ToList()
      );

      return vehicleJourneys;
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
      ObservableCollection<Journey> listSortedID = new ObservableCollection<Journey>(
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
