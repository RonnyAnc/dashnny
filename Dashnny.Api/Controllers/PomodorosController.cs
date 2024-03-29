using Microsoft.AspNetCore.Mvc;
using SlackNet;

namespace Dashnny.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PomodorosController : ControllerBase
{

	private readonly ILogger<PomodorosController> _logger;
	private readonly PomodoroRepository pomodoroRepository;
	private readonly SlackConfigurations slackConfigurations;
	private readonly StartPomodoro startPomodoro;

	public PomodorosController(ILogger<PomodorosController> logger, PomodoroRepository pomodoroRepository, StartPomodoro startPomodoro, SlackConfigurations slackOptions)
	{
		_logger = logger;
		this.pomodoroRepository = pomodoroRepository;
		this.slackConfigurations = slackOptions;
		this.startPomodoro = startPomodoro;
	}

	[HttpPost(Name = "StartPomodoro")]
	public async Task<StartPomodoroResponse> StartPomodoro(StartPomodoroRequest pomodoroRequest, CancellationToken cancellationToken)
	{
		var pomodoroResponse = await startPomodoro.Execute(pomodoroRequest);
		var slackConfiguration = pomodoroRequest.IsFromWork ? slackConfigurations.Work : slackConfigurations.Personal;
		var slackClient = new SlackApiClient(slackConfiguration.UserApiToken);
		await slackClient.Dnd.SetSnooze(pomodoroRequest.DurationInMinutes, cancellationToken);
		var botClient = slackClient.WithAccessToken(slackConfiguration.BotToken);
		var channel = pomodoroRequest.NotificationChannel ?? slackConfiguration.UserId;
		await botClient.Chat.PostMessage(new SlackNet.WebApi.Message { Channel = channel, Text = "Pomodoro Started!" });
		await botClient.Chat.ScheduleMessage(
			new SlackNet.WebApi.Message { Channel = channel, Text = "Pomodoro Finished!" },
			DateTime.Now.AddMinutes(pomodoroRequest.DurationInMinutes).AddSeconds(1));

		return new StartPomodoroResponse
		{
			Id = pomodoroResponse.Id
		};
	}

	[HttpGet(Name = "GetPomodoro")]
	public async Task<List<Pomodoro>> GetPomodoro([FromQuery] bool? isActive)
	{
		if (isActive.HasValue && isActive.Value)
		{
			return new List<Pomodoro> { await pomodoroRepository.FindActive() };
		}
		return await pomodoroRepository.GetAll();
	}
}
