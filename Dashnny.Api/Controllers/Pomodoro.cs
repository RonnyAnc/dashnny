using System.Text.Json.Serialization;

namespace Dashnny.Api.Controllers;

public class Pomodoro
{
	public string Id { get; }
	public string Key => Id;
	public string Label { get; }
	public DateTime StartTime { get; }
	public DateTime EndTime { get; }
	public int DurationInMinutes { get; }
	public int NumberInCycle { get; }
	public PomodoroStatus Status { get; private set; }
	public bool IsFromWork { get; }

	[JsonConstructor]
	public Pomodoro(string id, string label, DateTime startTime, DateTime endTime, int durationInMinutes, int numberInCycle, PomodoroStatus status, bool isFromWork)
	{
		Id = id;
		Label = label;
		StartTime = startTime;
		EndTime = endTime;
		DurationInMinutes = durationInMinutes;
		NumberInCycle = numberInCycle;
		Status = status;
		IsFromWork = isFromWork;
	}

	public Pomodoro(string label, DateTime startTime, DateTime endTime, int durationInMinutes, int numberInCycle, bool isFromWork)
	{
		Id = Guid.NewGuid().ToString();
		Label = label;
		StartTime = startTime;
		EndTime = endTime;
		DurationInMinutes = durationInMinutes;
		NumberInCycle = numberInCycle;
		Status = PomodoroStatus.ACTIVE;
		IsFromWork = isFromWork;
	}

	internal void Complete()
	{
		Status = PomodoroStatus.COMPLETED;
	}
}