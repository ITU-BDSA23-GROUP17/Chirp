using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public class CSVDatabase<T> : IDatabaseRepository<T>
{

    private static string path = "chirp_cli_db.csv";

    public IEnumerable<T> Read(int? limit = null)
    {
        List<T> list = new List<T>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {

        };
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, config))
        {
            if (limit != null )
            {
              try
              {
                for (int i = 0; i < limit; i++)
               {
                   csv.Read();
                   list.Add(csv.GetRecord<T>());
               }
              }
              catch (CsvHelper.MissingFieldException)
              {
                Console.WriteLine("There are not enough Cheeps see the ones we do have below:");
              }
               
            }
            else
            {
                while (csv.Read())
                {
                    list.Add(csv.GetRecord<T>());
                }
            }

        }
        return list;
    }

    // got a bit of help from source https://github.com/JoshClose/CsvHelper/issues/1726 as the library had been updated to use args
    public void Store(T record)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            Delimiter = ",",
            ShouldQuote = args => args.Row.Index == 1,
        };
        using (var stream = File.Open(path, FileMode.Append))
        using (var writer = new StreamWriter(stream))

        using (var csv = new CsvWriter(writer, config))
        {

            csv.WriteRecord(record);
            writer.Write("\n");
        }
    }
}

