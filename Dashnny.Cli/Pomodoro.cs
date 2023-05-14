using System.Text.Json.Serialization;

public class Pomodoro
{
	public string Id { get; }
	public string Key => Id;
	public string Label { get; }
	public bool IsActive { get; }
	public bool IsCompleted { get; }
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
}