using System.Data;
using Microsoft.Data.Sqlite;

// var sqlDBFilePath = "/tmp/chirp.db";
// @"SELECT * FROM message ORDER by message.pub_date desc";
// https://stackoverflow.com/questions/34691378/creating-sqlite-database-using-dump-file-programmatically
public class DBFacade
{
    string connectionString;

    public DBFacade()
    {
        InitDatabase();
    }

    void InitDatabase()
    {

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = GetEnvironmentVariable(),
        };

        connectionString = builder.ToString();



        using (var connection = new SqliteConnection(connectionString))
        {
            // open a connection
            connection.Open();

            var schemaCommand = connection.CreateCommand();

            // schema for the database
            string schemaFilePath = "./data/schema.sql";

            // Read contents of the file
            string schemaSql = File.ReadAllText(schemaFilePath);

            schemaCommand.CommandText = schemaSql;

            schemaCommand.ExecuteNonQuery();

            // now add data dump

            var dataDumpCommand = connection.CreateCommand();

            // schema for the database
            string dumpFilePath = "./data/dump.sql";

            // Read contents of the file
            string dumpSql = File.ReadAllText(dumpFilePath);

            dataDumpCommand.CommandText = dumpSql;

            dataDumpCommand.ExecuteNonQuery();

        }


    }

    private string GetEnvironmentVariable()
    {
        var dbpath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

        if (string.IsNullOrWhiteSpace(dbpath))
        {
            dbpath = Path.GetTempPath() + "chip.db";
            Console.WriteLine(dbpath);
        }

        return dbpath;

    }

    public List<CheepViewModel> DatabaseQuery(string sqlQuery)
    {
        var cheepList = new List<CheepViewModel>();

        // var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                // See https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader.getvalues?view=dotnet-plat-ext-7.0
                // for documentation on how to retrieve complete columns from query results
                cheepList.Add(new CheepViewModel(reader.GetString(0), reader.GetString(1), reader.GetString(2)));
            }

        }

        return cheepList;
    }
}

