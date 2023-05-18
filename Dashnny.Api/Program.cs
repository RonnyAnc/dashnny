using System.Text.Json;
using System.Text.Json.Serialization;
using Dashnny.Api.Controllers;
using SlackNet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
	options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	options.JsonSerializerOptions.WriteIndented = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSlackNet(c => c.UseApiToken(builder.Configuration["SLACK_BOT_API_TOKEN"]));
builder.Services.AddPomodoroFeatures(builder.Configuration);
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSlackNet(c => c.UseSigningSecret(builder.Configuration["SLACK_SIGNING_SECRET"]).MapToPrefix("api/slack"));

app.MapControllers();

app.Run();

public partial class Program { }