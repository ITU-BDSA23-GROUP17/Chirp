using System;
using System.IO;


if (args[0] == "read") {

    try
        {
            // Open the text file using a stream reader.
            using (var sr = new StreamReader("chirp_cli_db.csv"))
            {
                // Read the stream as a string, and write the string to the console.
                Console.WriteLine(sr.ReadToEnd());
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

} else if (args[0] == "cheep") {
    Console.WriteLine("No");
}