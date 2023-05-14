using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class PomodoroCommandExtensions
{
	public static RootCommand AddPomodorosFeatures(this RootCommand rootCommand)
	{
		var pomodoros = new Command("pomodoros", "Pomodoros features");
		pomodoros.AddStartPomodoroCommand();
		pomodoros.AddGetAllPomodorosCommand();
		pomodoros.AddGetActivePomodoro();
		rootCommand.Add(pomodoros);
		return rootCommand;
	}

	private static Command AddStartPomodoroCommand(this Command parentCommand)
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
			using var response = await new HttpClient().PostAsync("https://localhost:7248/Pomodoros", content);
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine(await response.Content.ReadAsStringAsync());
			}
			Console.WriteLine("⏳ Pomodoro Started");
		}, durationOption, labelOption);
		parentCommand.Add(startPomodoro);
		return parentCommand;
	}

	private static Command AddGetAllPomodorosCommand(this Command parentCommand)
	{
		var listPomodoros = new Command("list", "List pomodoros");
		listPomodoros.SetHandler(async () =>
		{
			using var response = await new HttpClient().GetAsync("https://localhost:7248/Pomodoros");
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine(await response.Content.ReadAsStringAsync());
			}
			var serializedPomodoros = await response.Content.ReadAsStringAsync();
			var pomodoros = JsonSerializer.Deserialize<List<Pomodoro>>(serializedPomodoros, new JsonSerializerOptions
			{
				NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			});
			PrintLine();
			PrintRow("Id", "Label", "Start Time", "End Time", "Is Active", "Is Completed");
			PrintLine();
			foreach (var pomodoro in pomodoros)
			{
				PrintRow(
					$"{pomodoro.Id}",
					$"{pomodoro.Label}",
					$"{pomodoro.StartTime}",
					$"{pomodoro.EndTime}",
					$"{pomodoro.IsActive}",
					$"{pomodoro.IsCompleted}");
			}

		});
		parentCommand.Add(listPomodoros);
		return parentCommand;
	}

	private static Command AddGetActivePomodoro(this Command parentCommand)
	{
		var getPomodoro = new Command("get", "Get a pomodoro");
		var isActiveOption = new Option<bool>("--active", "Get the active pomodoro");
		getPomodoro.AddOption(isActiveOption);
		getPomodoro.SetHandler(async (_) =>
		{
			using var response = await new HttpClient().GetAsync($"https://localhost:7248/Pomodoros?isActive=true");
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine(await response.Content.ReadAsStringAsync());
			}
			var serializedPomodoros = await response.Content.ReadAsStringAsync();
			Console.WriteLine(serializedPomodoros);

		}, isActiveOption);
		parentCommand.Add(getPomodoro);
		return parentCommand;
	}

	static void PrintLine()
	{
		Console.WriteLine(new string('-', tableWidth));
	}
	static int tableWidth = 150;
	static void PrintRow(params string[] columns)
	{
		int width = (tableWidth - columns.Length) / columns.Length;
		string row = "|";

		foreach (string column in columns)
		{
			row += AlignCentre(column, width) + "|";
		}

		Console.WriteLine(row);
	}

	static string AlignCentre(string text, int width)
	{
		text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

		if (string.IsNullOrEmpty(text))
		{
			return new string(' ', width);
		}
		else
		{
			return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
		}
	}
}