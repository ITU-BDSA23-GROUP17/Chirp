using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
    string path = "chirp_cli_db.csv";

    public IEnumerable<T> Read(int? limit = null)
    {
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<T>().ToList();
        }
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
       
       using (var csv = new CsvWriter(writer,  config))
        {
            
            csv.WriteRecord(record);
            writer.Write("\n");
      }
    }
}

