var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres(
    "postgres", 
    builder.AddParameter("username", true), 
    builder.AddParameter("password", true));
var postgresdb = postgres.AddDatabase("Bitlog");


builder
    .AddProject<Projects.Bitlog_WebAPI>("WebApi")
    .WithReference(postgresdb);

builder.Build().Run();
