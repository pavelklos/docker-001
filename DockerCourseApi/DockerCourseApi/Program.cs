var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x.AllowAnyOrigin());

// app.MapGet("/", () => "Hello World!");
app.MapGet("/podcasts", () => new List<string>
{
    "Unhandled Exception Podcast",
    "Developer Weekly Podcast",
    "The Stack Overflow Podcast",
    "The Hanselminutes Podcast",
    "The .NET Rocks Podcast",
    "The Azure Podcast",
    "The AWS Podcast",
    "The Rabbit Hole Podcast",
    "The .NET Core Podcast",
});

app.Run();
