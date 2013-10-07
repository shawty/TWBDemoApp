using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using DummyData.Entities;

namespace DummyData
{
  public class DataApi
  {
    private const string _databaseName = "TestData.s3db";

    public int AddPerson(Person newPerson)
    {
      CheckAndCreatePersonTable();
      long newId;

      using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
      {
        connection.Open();
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
          command.CommandText = "INSERT INTO Persons (FirstName, Surname, EMail, Age, Salary) VALUES($FirstName,$Surname,$EMail,$Age,$Salary)";
          command.Parameters.AddWithValue("$FirstName", newPerson.FirstName);
          command.Parameters.AddWithValue("$Surname", newPerson.Surname);
          command.Parameters.AddWithValue("$EMail", newPerson.EMail);
          command.Parameters.AddWithValue("$Age", newPerson.Age);
          command.Parameters.AddWithValue("$Salary", newPerson.Salary);
          command.CommandType = CommandType.Text;
          command.ExecuteNonQuery();

          command.CommandText = "SELECT last_insert_rowid()";
          newId = (long)command.ExecuteScalar();
        }
        connection.Close();
      }
      return (int)newId;
    }

    public void DeletePersonById(int recordId)
    {
      CheckAndCreatePersonTable();

      using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
      {
        connection.Open();
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
          command.CommandText = "DELETE FROM Persons WHERE RecordId = $RecordId";
          command.Parameters.AddWithValue("$RecordId", recordId);
          command.CommandType = CommandType.Text;
          command.ExecuteNonQuery();
        }
        connection.Close();
      }
    }

    public void UpdatePerson(Person personToUpdate)
    {
      CheckAndCreatePersonTable();

      using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
      {
        connection.Open();
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
          command.CommandText = "UPDATE Persons SET FirstName = $FirstName, Surname = $Surname, EMail = $EMail, Age = $Age, Salary = $Salary WHERE RecordId = $RecordId";
          command.Parameters.AddWithValue("$RecordId", personToUpdate.RecordId);
          command.Parameters.AddWithValue("$FirstName", personToUpdate.FirstName);
          command.Parameters.AddWithValue("$Surname", personToUpdate.Surname);
          command.Parameters.AddWithValue("$EMail", personToUpdate.EMail);
          command.Parameters.AddWithValue("$Age", personToUpdate.Age);
          command.Parameters.AddWithValue("$Salary", personToUpdate.Salary);
          command.CommandType = CommandType.Text;
          command.ExecuteNonQuery();
        }
        connection.Close();
      }
    }

    public Person FindPersonById(int recordId)
    {
      CheckAndCreatePersonTable();
      Person result = null;

      using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
      {
        connection.Open();
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
          command.CommandText = "SELECT * FROM Persons WHERE RecordId = $RecordId";
          command.Parameters.AddWithValue("$RecordId", recordId);
          command.CommandType = CommandType.Text;
          using(SQLiteDataReader reader = command.ExecuteReader())
          {
            if(reader.HasRows)
            {
              reader.Read();
              result = GetPersonRecord(reader);
            }
          }
        }
        connection.Close();
      }

      return result;
    }

    public List<Person>GetAllPersons()
    {
      CheckAndCreatePersonTable();
      List<Person> results = new List<Person>();

      using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
      {
        connection.Open();
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
          command.CommandText = "SELECT * FROM Persons";
          command.CommandType = CommandType.Text;
          using (SQLiteDataReader reader = command.ExecuteReader())
          {
            if (reader.HasRows)
            {
              while(reader.Read())
              {
                results.Add(GetPersonRecord(reader));
              }
            }
          }
        }
        connection.Close();
      }
      return results;
    }

    private void CheckAndCreatePersonTable()
    {
      if (TableExists("Persons")) return;
      CreatePersonTable();
      Seed();
    }

    private static Person GetPersonRecord(SQLiteDataReader reader)
    {
      return new Person
                 {
                   RecordId = Convert.ToInt32(reader["RecordId"]),
                   FirstName = Convert.ToString(reader["FirstName"]),
                   Surname = Convert.ToString(reader["Surname"]),
                   EMail = Convert.ToString(reader["EMail"]),
                   Age = Convert.ToInt32(reader["Age"]),
                   Salary = Convert.ToDecimal(reader["Salary"])
                 };
    }

    private static string GetConnectionString()
    {
      string binPath = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath);
      SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder {DataSource = binPath + "\\" + _databaseName};
      return csb.ConnectionString;
    }

    private static bool TableExists(string tableName)
    {
      bool result;
      string dbConnectionString = GetConnectionString();

      using (SQLiteConnection connection = new SQLiteConnection(dbConnectionString))
      {
        connection.Open();
        string sql = string.Format("SELECT name FROM sqlite_master WHERE name='{0}'", tableName);
        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
        {
          using (SQLiteDataReader reader = command.ExecuteReader())
          {
            result = reader.HasRows;
          }
        }
        connection.Close();
      }

      return result;
    }

    private static void CreatePersonTable()
    {
      string dbConnectionString = GetConnectionString();

      using (SQLiteConnection connection = new SQLiteConnection(dbConnectionString))
      {
        connection.Open();
        using (SQLiteCommand command = new SQLiteCommand(connection))
        {
          command.CommandText = "CREATE TABLE Persons (RecordId integer primary key autoincrement not null, FirstName varchar(50), Surname varchar(50), EMail varchar(100), Age integer not null default '16', Salary numeric not null default '15000.00')";
          command.CommandType = CommandType.Text;
          command.ExecuteNonQuery();
        }
        connection.Close();
      }
    }

    private void Seed()
    {
      AddPerson(new Person { FirstName = "Peter1", Surname = "Shaw1", EMail = "peter.shaw@lidnug.org", Age = 21, Salary = 1000000, RecordId = 0 });
      AddPerson(new Person { FirstName = "Peter2", Surname = "Shaw2", EMail = "peter.shaw@lidnug.org", Age = 21, Salary = 2000000, RecordId = 0 });
      AddPerson(new Person { FirstName = "Peter3", Surname = "Shaw3", EMail = "peter.shaw@lidnug.org", Age = 21, Salary = 3000000, RecordId = 0 });
      AddPerson(new Person { FirstName = "Peter4", Surname = "Shaw4", EMail = "peter.shaw@lidnug.org", Age = 21, Salary = 4000000, RecordId = 0 });
      AddPerson(new Person { FirstName = "Peter5", Surname = "Shaw5", EMail = "peter.shaw@lidnug.org", Age = 21, Salary = 5000000, RecordId = 0 });
    }

  }
}
