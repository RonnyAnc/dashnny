using Microsoft.AspNetCore.Mvc;
using SlackNet;

namespace Dashnny.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PomodorosController : ControllerBase
{

	private readonly ILogger<PomodorosController> _logger;
	private readonly PomodoroRepository pomodoroRepository;
	private readonly ISlackApiClient slackClient;
	private readonly SlackOptions slackOptions;

	public PomodorosController(ILogger<PomodorosController> logger, PomodoroRepository pomodoroRepository, ISlackApiClient slackClient, SlackOptions slackOptions)
	{
		_logger = logger;
		this.pomodoroRepository = pomodoroRepository;
		this.slackClient = slackClient;
		this.slackOptions = slackOptions;
	}

	[HttpPost(Name = "StartPomodoro")]
	public async Task<StartPomodoroResponse> StartPomodoro(StartPomodoroRequest pomodoroRequest, CancellationToken cancellationToken)
	{
		Pomodoro activePomodoro = await pomodoroRepository.FindActive();
		if (activePomodoro != null)
		{
			throw new Exception("An active pomodoro already exists");
		}
		var pomodoro = new Pomodoro(
			pomodoroRequest.Label,
			pomodoroRequest.StartTime,
			pomodoroRequest.StartTime.AddMinutes(pomodoroRequest.DurationInMinutes),
			pomodoroRequest.DurationInMinutes,
			pomodoroRequest.NumberInCycle
		);
		await pomodoroRepository.Create(pomodoro);
		await slackClient.Dnd.SetSnooze(pomodoro.DurationInMinutes, cancellationToken);
		var botClient = slackClient.WithAccessToken(slackOptions.UserBotToken);
		await botClient.Chat.PostMessage(new SlackNet.WebApi.Message { Channel = slackOptions.UserId, Text = "Pomodoro Started!" });
		await botClient.Chat.ScheduleMessage(
			new SlackNet.WebApi.Message { Channel = slackOptions.UserId, Text = "Pomodoro Finished!" },
			DateTime.Now.AddMinutes(pomodoro.DurationInMinutes).AddSeconds(1));

		return new StartPomodoroResponse
		{
			Id = pomodoro.Id
		};
	}

	[HttpGet(Name = "GetPomodoro")]
	public async Task<Pomodoro> GetPomodoro()
	{
		return await pomodoroRepository.FindActive();
	}
}

public class StartPomodoroRequest
{
	public string Label { get; set; }
	public DateTime StartTime { get; set; }
	public int DurationInMinutes { get; set; }
	public int NumberInCycle { get; set; }
}

public class StartPomodoroResponse
{
	public string Id { get; set; }
}
