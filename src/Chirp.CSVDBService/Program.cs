
using SimpleDB;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var database = CSVDatabase<Cheep>.Instance;


app.MapGet("/cheeps", (int? limit) => database.Read(limit));
app.MapPost("/cheep", (Cheep cheep) => database.Store(cheep));



app.Run();

public record Cheep(string Author, string Message, long Timestamp);


 
