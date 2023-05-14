public class SlackConfigurations
{
	public SlackConfiguration Personal { get; set; }
	public SlackConfiguration Work { get; set; }
}

public class SlackConfiguration
{
	public string UserApiToken { get; set; }
	public string BotToken { get; set; }
	public string UserId { get; set; }
}