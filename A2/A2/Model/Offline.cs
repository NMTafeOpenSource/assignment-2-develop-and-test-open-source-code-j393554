using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Offline
  {
    private JsonSerializer serializer = new JsonSerializer();

    public List<dynamic> List;

    public string LoadStatus;
    public string LoadStatusShort;
    public string FileURI;
    private string FileName;
    
    /// <summary>
    /// Loads a resource if it exists. Creates it if it does not.
    /// </summary>
    /// <param name="FileName">Filename to look up or create</param>
    /// <returns>True if successful, false otherwise.</returns>
    public bool Load( string FileName )
    {
      bool Loaded = false;
      bool Created = false;
      this.FileName = FileName;

      // Create File if it doesn't exist from an empty list
      if (!File.Exists(FileName))
      {
        using (StreamWriter file = File.CreateText(FileName))
        {
          List = new List<dynamic>();
          serializer.Serialize(file, List);
          Created = true;
        }
      }

      // Load file
      using (StreamReader file = File.OpenText(FileName))
      {
        List = (List<dynamic>)serializer.Deserialize(file, typeof(List<dynamic>));

        Loaded = true;

        FileInfo TheFile = new FileInfo(FileName);
        FileURI = TheFile.FullName;
      }

      // Prime status messages
      if (Created == false && Loaded)
      {
        LoadStatusShort = "NORMAL_LOAD";
        LoadStatus = "Loaded at " + FileURI;
      }
      else if (Created && Loaded)
      {
        LoadStatusShort = "CREATE_LOAD";
        LoadStatus = "Created and Loaded at " + FileURI;
      }
      else
      {
        LoadStatusShort = "ERROR_LOAD";
        LoadStatus = "Error Loading " + FileName;
      }

      return Loaded;
    }

    /// <summary>
    /// Saves the companies in the list into the file set in the class.
    /// </summary>
    public void Save( dynamic List )
    {
      using (StreamWriter file = File.CreateText(FileName))
      {
        serializer.Serialize(file, List);
      }

      // Set status message
      LoadStatusShort = "SAVED";
      LoadStatus = "Saved at " + FileURI;
    }
  }
}
