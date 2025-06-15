<Query Kind="Statements">
  <NuGetReference>StackExchange.Redis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
</Query>

var redis = ConnectionMultiplexer.Connect("localhost");

var db = redis.GetDatabase();

var value = "Hello dotnet";

db.StringSet("WelcomeMsg", value);

db.StringGet("WelcomeMsg").Dump();
