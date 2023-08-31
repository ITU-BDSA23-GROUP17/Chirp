using Microsoft.VisualBasic.FileIO;

using (TextFieldParser parser = new TextFieldParser(@"C:\Users\Hanan\Documents\Chirp.CLI\chirp_cli_db.csv"))
{
    
    parser.TextFieldType = FieldType.Delimited;
    parser.SetDelimiters(",");
    while (!parser.EndOfData) 
    {
        //Processing row
        string[] fields = parser.ReadFields();
        foreach (string field in fields) 
        {
            Console.WriteLine(field);
        }
    }
}