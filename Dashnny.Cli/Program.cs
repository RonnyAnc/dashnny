// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
	.AddEnvironmentVariables()
	.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "clisettings.json"), true)
	.Build();

var rootCommand = new RootCommand("Dashnny base command");

rootCommand.AddPomodorosFeatures(configuration);

await rootCommand.InvokeAsync(args);