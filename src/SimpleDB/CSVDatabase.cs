using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

/*
    This class is refactored to follow the Singleton Pattern. 
    The refactoring is based upon the 'First version - not thread-safe'
    -version of the article 'Implementing the Singleton Pattern in C#',
    the companion website for 'C# in Depth' by Jon Skeet.
    Source: https://csharpindepth.com/Articles/Singleton#unsafe
*/
public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{

    private static CSVDatabase<T>? instance = null;

    public static CSVDatabase<T> Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CSVDatabase<T>();
            }
            return instance;
        }
    }

    private CSVDatabase()
    {
    }

    string path = "chirp_cli_db.csv";

    public IEnumerable<T> Read(int? limit = null)
    {
        List<T> list = new List<T>();
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            if (limit != null)
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

