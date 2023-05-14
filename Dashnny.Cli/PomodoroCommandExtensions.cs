using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dashnny.Api.Controllers;
using Microsoft.Extensions.Configuration;

public static class PomodoroCommandExtensions
{
	public static RootCommand AddPomodorosFeatures(this RootCommand rootCommand, Microsoft.Extensions.Configuration.IConfigurationRoot configuration)
	{
		var pomodoros = new Command("pomodoros", "Pomodoros features");
		var dashnnyConfiguration = new DashnnyConfiguration(
			configuration.GetValue<string>("DASHNNY_API_URL"),
			configuration.GetValue<string>("DASHNNY_API_KEY"));
		pomodoros.AddStartPomodoroCommand(dashnnyConfiguration);
		pomodoros.AddGetAllPomodorosCommand(dashnnyConfiguration);
		pomodoros.AddGetActivePomodoro(dashnnyConfiguration);
		rootCommand.Add(pomodoros);
		return rootCommand;
	}

	private static Command AddStartPomodoroCommand(this Command parentCommand, DashnnyConfiguration dashnnyConfiguration)
	{
		var startPomodoro = new Command("start", "Start a pomodoro");
		var durationOption = new Option<int>(name: "--duration", description: "Pomodoro duration in minutes");
		startPomodoro
			.AddOption(durationOption);
		var labelOption = new Option<string>(name: "--label", description: "Label for pomodoro");
		startPomodoro
			.AddOption(labelOption);
		var workOption = new Option<bool>(name: "--work", description: "Mark pomodoro as a work pomodoro and send to different work slack workspace if configured");
		startPomodoro
			.AddOption(workOption);
		startPomodoro.SetHandler(async (duration, label, work) =>
		{
			var startPomodoroRequest = new StartPomodoroRequest
			{
				DurationInMinutes = duration,
				Label = label,
				NumberInCycle = 1,
				StartTime = DateTime.Now,
				IsFromWork = work
			};
			var content = Serialize(startPomodoroRequest);
			using var request = new HttpRequestMessage(HttpMethod.Post, $"{dashnnyConfiguration.DashnnyApiUrl}/Pomodoros");
			request.Headers.TryAddWithoutValidation("X-Space-App-Key", dashnnyConfiguration.DashnnyApiKey);
			request.Content = content;
			using var response = await new HttpClient().SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine(response.StatusCode);
				var error = await response.Content.ReadAsStringAsync();
				Console.WriteLine(error);
				throw new Exception(error);
			}
			Console.WriteLine("⏳ Pomodoro Started");
		}, durationOption, labelOption, workOption);
		parentCommand.Add(startPomodoro);
		return parentCommand;
	}

	private static StringContent Serialize<T>(T startPomodoroRequest)
	{
		var serializedObject = JsonSerializer.Serialize(startPomodoroRequest, new JsonSerializerOptions
		{
			NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true
		});
		return new StringContent(serializedObject, Encoding.UTF8, "application/json");
	}

	private static Command AddGetAllPomodorosCommand(this Command parentCommand, DashnnyConfiguration dashnnyConfiguration)
	{
		var listPomodoros = new Command("list", "List pomodoros");
		listPomodoros.SetHandler(async () =>
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, $"{dashnnyConfiguration.DashnnyApiUrl}/Pomodoros");
			request.Headers.TryAddWithoutValidation("X-Space-App-Key", dashnnyConfiguration.DashnnyApiKey);
			using var response = await new HttpClient().SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine(response.StatusCode);
				var error = await response.Content.ReadAsStringAsync();
				Console.WriteLine(error);
				throw new Exception(error);
			}
			var serializedPomodoros = await response.Content.ReadAsStringAsync();
			Console.WriteLine(serializedPomodoros);
		});
		parentCommand.Add(listPomodoros);
		return parentCommand;
	}

	private static Command AddGetActivePomodoro(this Command parentCommand, DashnnyConfiguration dashnnyConfiguration)
	{
		var getPomodoro = new Command("get", "Get a pomodoro");
		var isActiveOption = new Option<bool>("--active", "Get the active pomodoro");
		getPomodoro.AddOption(isActiveOption);
		getPomodoro.SetHandler(async (_) =>
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, $"{dashnnyConfiguration.DashnnyApiUrl}/Pomodoros?isActive=true");
			request.Headers.TryAddWithoutValidation("X-Space-App-Key", dashnnyConfiguration.DashnnyApiKey);
			using var response = await new HttpClient().SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync();
				Console.WriteLine(error);
				throw new Exception(error);
			}
			var serializedPomodoros = await response.Content.ReadAsStringAsync();
			Console.WriteLine(serializedPomodoros);

		}, isActiveOption);
		parentCommand.Add(getPomodoro);
		return parentCommand;
	}
}