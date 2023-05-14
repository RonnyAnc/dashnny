// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var rootCommand = new RootCommand("Dashnny base command");

rootCommand.AddPomodorosFeatures();

await rootCommand.InvokeAsync(args);