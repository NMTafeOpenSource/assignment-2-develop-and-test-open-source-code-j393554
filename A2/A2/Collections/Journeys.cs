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
      LoadJourneys();
    }

    public void LoadJourneys()
    {
      // TODO: Load Journeys with SQL Statement
    }

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

    public void EditJourney( Journey journey, DateTime StartDate, DateTime EndDate, int StartOdometer, int EndOdometer)
    {
      var FindJourney = List.FirstOrDefault( j => j.Id == journey.Id );

      if ( FindJourney != null )
      {
        FindJourney.StartDate = StartDate;
        FindJourney.EndDate = EndDate;
        FindJourney.StartOdometer = StartOdometer;
        FindJourney.EndOdometer = EndOdometer;
      }
    }

    public void DeleteJourney( Journey journey )
    {
      List.Remove( journey );
    }

    public Journey RecentJourney( Vehicle vehicle )
    {
      Journey RecentJourney = null;

      if ( List.Count > 0 )
      {
        // Group Services by Vehicle ID.
        ObservableCollection<Journey> VehicleJourneys = ByVehicle( vehicle );
      
        if ( VehicleJourneys.Count > 0 )
        {
          RecentJourney = VehicleJourneys[0];
        }
      }

      return RecentJourney;
    }

    public ObservableCollection<Journey> ByVehicle( Vehicle vehicle )
    {
      ObservableCollection<Journey> VehicleJourneys = new ObservableCollection<Journey>(
        List
          .Where(j => j.VehicleId == vehicle.Id)
          .OrderBy(j => j.EndDate)
          .Reverse()
          .ToList()
      );

      return VehicleJourneys;
    }

    public int FindId()
    {
      int id;
      int tail;

      tail = List.Count;

      if (tail == 0)
      {
        id = 1;
      }
      else
      {
        tail--;
        id = List[tail].Id + 1;
      }

      return id;
    }
  }
}
