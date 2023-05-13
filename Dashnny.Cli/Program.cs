// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var rootCommand = new RootCommand("Dashnny base command");

Command pomodoros = AddPomodorosFeatures();
rootCommand.Add(pomodoros);

await rootCommand.InvokeAsync(args);

static Command AddPomodorosFeatures()
{
	var pomodoros = new Command("pomodoros", "Pomodoros features");
	AddStartPomodoroUseCase(pomodoros);
	return pomodoros;

	static void AddStartPomodoroUseCase(Command pomodoros)
	{
		var startPomodoro = new Command("start", "Start a pomodoro");
		var durationOption = new Option<int>(name: "--duration", description: "Pomodoro duration in minutes");
		startPomodoro
			.AddOption(durationOption);
		var labelOption = new Option<string>(name: "--label", description: "Label for pomodoro");
		startPomodoro
			.AddOption(labelOption);
		startPomodoro.SetHandler(async (duration, label) =>
		{
			var startPomodoroRequest = new StartPomodoroRequest
			{
				DurationInMinutes = duration,
				Label = label,
				NumberInCycle = 1,
				StartTime = DateTime.Now
			};
			var serializedRequest = JsonSerializer.Serialize(startPomodoroRequest, new JsonSerializerOptions
			{
				NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			});
			var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
			var response = await new HttpClient().PostAsync("https://localhost:7248/Pomodoros", content);
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine(await response.Content.ReadAsStringAsync());
			}
			Console.WriteLine("⏳ Pomodoro Started");
		}, durationOption, labelOption);
		pomodoros.Add(startPomodoro);
	}
}