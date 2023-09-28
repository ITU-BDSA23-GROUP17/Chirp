using System.Data;
using Microsoft.Data.Sqlite;

// var sqlDBFilePath = "/tmp/chirp.db";
// @"SELECT * FROM message ORDER by message.pub_date desc";

public class DBFacade
{


    public static List<CheepViewModel> DatabaseQuery(string sqlQuery)
    {
        var cheepList = new List<CheepViewModel>();
        var sqlDBFilePath = "/tmp/chirp.db";
        // var sqlQuery = @"SELECT * FROM message ORDER by message.pub_date desc";

        using (var connection = new SqliteConnection($"Data Source={sqlDBFilePath}"))
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

