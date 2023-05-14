namespace Dashnny.Api.Controllers;

public class StartPomodoro
{
	private readonly PomodoroRepository pomodoroRepository;

	public StartPomodoro(PomodoroRepository pomodoroRepository)
	{
		this.pomodoroRepository = pomodoroRepository;
	}

	public async Task<StartPomodoroResponse> Execute(StartPomodoroRequest pomodoroRequest)
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
		return new StartPomodoroResponse
		{
			Id = pomodoro.Id
		};
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