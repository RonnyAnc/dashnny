using System.Net.Http.Headers;
using Dashnny.Api;
using Dashnny.Api.Controllers;
using SlackNet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var dbApiConnection = new DBApiConnection(
	new Uri(builder.Configuration["DASHNNY_DB_URL"]), builder.Configuration["DASHNNY_DB_API_KEY"]);
builder.Services.AddSingleton(dbApiConnection);
builder.Services.AddSingleton(new SlackOptions
{
	UserApiToken = builder.Configuration["SLACK_USER_API_TOKEN"],
	UserBotToken = builder.Configuration["SLACK_BOT_API_TOKEN"],
	UserId = builder.Configuration["SLACK_USER_ID"]
});
builder.Services.AddSlackNet(c => c.UseApiToken(builder.Configuration["SLACK_USER_API_TOKEN"]));
builder.Services.AddHttpClient<PomodoroRepository>(client =>
{
	client.BaseAddress = dbApiConnection.DbUri;
});
builder.Services.AddHostedService<PomodoroStatusChecker>();
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

app.MapControllers();

app.Run();

public partial class Program { }