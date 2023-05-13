public class StartPomodoroRequest
{
	public string Label { get; set; }
	public DateTime StartTime { get; set; }
	public int DurationInMinutes { get; set; }
	public int NumberInCycle { get; set; }
}