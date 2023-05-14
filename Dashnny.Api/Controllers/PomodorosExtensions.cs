using SlackNet.AspNetCore;

namespace Dashnny.Api.Controllers;

public static class ServicesExtensions
{
	public static IServiceCollection AddPomodoroFeatures(this IServiceCollection services, IConfiguration configuration)
	{
		var dbApiConnection = new DBApiConnection(
		new Uri(configuration["DASHNNY_DB_URL"]), configuration["DASHNNY_DB_API_KEY"]);
		services.AddSingleton(dbApiConnection);
		services.AddSingleton(new SlackOptions
		{
			UserApiToken = configuration["SLACK_USER_API_TOKEN"],
			UserBotToken = configuration["SLACK_BOT_API_TOKEN"],
			UserId = configuration["SLACK_USER_ID"]
		});

		services.AddScoped<StartPomodoro>();
		services.AddSlackNet(c => c.UseApiToken(configuration["SLACK_USER_API_TOKEN"]));
		services.AddHttpClient<PomodoroRepository>(client =>
		{
			client.BaseAddress = dbApiConnection.DbUri;
		});
		services.AddHostedService<PomodoroStatusChecker>();
		return services;
	}
}