// using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin());

app.MapGet("/podcasts", async () =>
{
    // var db = new SqlConnection("Server=tcp:localhost;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
    // var db = new SqlConnection("Server=tcp:database;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
    var connectionString = "Server=tcp:database,1433;Initial Catalog=podcasts;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

    using var db = new SqlConnection(connectionString);

    var podcasts = (await db.QueryAsync<Podcast>("SELECT * FROM Podcasts")).Select(x => x.Title);

    return podcasts;

    // return new List<string>
    // {
    //     "Unhandled Exception Podcast",
    //     "Developer Weekly Podcast",
    //     "The Stack Overflow Podcast",
    //     "The Hanselminutes Podcast",
    //     "The .NET Rocks Podcast",
    //     "The Azure Podcast",
    //     "The AWS Podcast",
    //     "The Rabbit Hole Podcast",
    //     "The .NET Core Podcast",
    // };
});

// Verify that DNS entry database is working
app.MapGet("/test-connection", async () =>
{
    try 
    {
        var connectionString = "Server=tcp:database,1433;Initial Catalog=master;Persist Security Info=False;User ID=sa;Password=dotnet#123;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connection Timeout=10;";
        
        using var db = new SqlConnection(connectionString);
        await db.OpenAsync();
        
        return "✅ Connection to 'database' successful!";
    }
    catch (Exception ex)
    {
        return $"❌ Connection failed: {ex.Message}";
    }
});

app.Run();

record Podcast(Guid Id, string Title);

// Make Program accessible to tests
public partial class Program { }