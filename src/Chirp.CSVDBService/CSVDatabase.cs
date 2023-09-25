using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using CsvHelper;
using CsvHelper.Configuration;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private static CSVDatabase<T>? instance = null;
    private static readonly object lockObject = new object();

    private CSVDatabase()
    {
        // Private constructor
    }

    public static CSVDatabase<T> Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new CSVDatabase<T>();
                        instance.FetchDataFromInternet();
                    }
                }
            }
            return instance;
        }
    }

    string path = "chirp_cli_db.csv";

    private void FetchDataFromInternet()
    {
        if (!File.Exists(path))
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync("https://raw.githubusercontent.com/ITU-BDSA23-GROUP17/Chirp/main/data/chirp_cli_db.csv").Result;
                    response.EnsureSuccessStatusCode();

                    using (var stream = response.Content.ReadAsStreamAsync().Result)
                    using (var reader = new StreamReader(stream))
                    {
                        // Save the fetched data to the local file
                        File.WriteAllText(path, reader.ReadToEnd());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching data from the internet: {ex.Message}");
                }
            }
        }
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        // Ensure data is fetched from the internet on the first use
        if (!File.Exists(path))
        {
            FetchDataFromInternet();
        }

        // Rest of the Read method remains the same...
        List<T> list = new List<T>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);

        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, config))
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

    public void Store(T record)
    {
        // Ensure data is fetched from the internet on the first use
        if (!File.Exists(path))
        {
            FetchDataFromInternet();
        }

        // Rest of the Store method remains the same...
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
