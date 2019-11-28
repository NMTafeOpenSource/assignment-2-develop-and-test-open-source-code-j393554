using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  public class Database
  {
    private string server;
    private string database;
    private string user;
    private string password;
    private int port;

    private string connectionString;
    public MySqlConnection Connection { get; private set; }
    public MySqlException LastException { get; private set; }

    public bool Error { get; private set; }

    /// <summary>
    /// Database resource class using MySQL (squirrel)
    /// </summary>
    /// <param name="database">Database?</param>
    /// <param name="user">Username?</param>
    /// <param name="password">Umm cmon do I have to explain what you need for a database connection. Of course a password.</param>
    /// <param name="server">... 127.0.0.1?</param>
    /// <param name="port">Port number in case you're using a different one. Defaults to 3306</param>
    public Database( string database, string user, string password, string server = "localhost", int port = 3306 )
    {
      this.server = server;
      this.database = database;
      this.user = user;
      this.password = password;
      this.port = port;

      // Constructs connection string.
      connectionString = $"server={server};user={user};database={database};port={port};password={password};";

      // And the connection object
      Connection = new MySqlConnection( connectionString );

      // Simple connection test
      if ( Open() )
      {
        Close();
      }
    }

    /// <summary>
    /// Opens a connection to the database
    /// </summary>
    /// <returns>True if successful. False otherwise. Exception can be looked into on LastException</returns>
    public bool Open()
    {
      try
      {
        Connection.Open();
        return true;
      }
      catch (MySqlException exception)
      {
        LastException = exception;
      }

      return false;
    }

    /// <summary>
    /// Closes a connection to the database
    /// </summary>
    /// <returns>True if successful. False otherwise. Exception can be looked into on LastException</returns>
    public bool Close()
    {
      try
      {
        Connection.Close();
        return true;
      }
      catch (MySqlException exception)
      {
        LastException = exception;
      }

      return false;
    }
  }
}
