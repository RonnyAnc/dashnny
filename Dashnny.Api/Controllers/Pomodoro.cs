using System.Text.Json.Serialization;

namespace Dashnny.Api.Controllers;

public class Pomodoro
{
	public string Id { get; }
	public string Key => Id;
	public string Label { get; }
	public bool IsActive { get; private set; }
	public bool IsCompleted { get; private set; }
	public DateTime StartTime { get; }
	public DateTime EndTime { get; }
	public int DurationInMinutes { get; }
	public int NumberInCycle { get; }

	[JsonConstructor]
	public Pomodoro(string id, string label, DateTime startTime, DateTime endTime, int durationInMinutes, int numberInCycle, bool isActive, bool isCompleted)
	{
		Id = id;
		Label = label;
		StartTime = startTime;
		IsActive = isActive;
		IsCompleted = isCompleted;
		EndTime = endTime;
		DurationInMinutes = durationInMinutes;
		NumberInCycle = numberInCycle;
	}

	public Pomodoro(string label, DateTime startTime, DateTime endTime, int durationInMinutes, int numberInCycle)
	{
		Id = Guid.NewGuid().ToString();
		Label = label;
		StartTime = startTime;
		IsActive = true;
		IsCompleted = false;
		EndTime = endTime;
		DurationInMinutes = durationInMinutes;
		NumberInCycle = numberInCycle;
	}

	internal void Complete()
	{
		IsActive = false;
		IsCompleted = true;
	}
}