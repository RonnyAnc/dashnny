using SlackNet.AspNetCore;

namespace Dashnny.Api.Controllers;

public static class ServicesExtensions
{
	public static IServiceCollection AddPomodoroFeatures(this IServiceCollection services, IConfiguration configuration)
	{
		var dashnnyApiUrl = configuration.GetValue("DASHNNY_DB_URL", "");
		var dashnnyApiKey = configuration.GetValue("DETA_PROJECT_KEY", "");
		var dbApiConnection = new DBApiConnection(
			new Uri(dashnnyApiUrl), dashnnyApiKey);
		services.AddSingleton(dbApiConnection);
		services.AddSingleton(new SlackConfigurations
		{
			Personal = new SlackConfiguration
			{
				UserApiToken = configuration["SLACK_USER_API_TOKEN"],
				BotToken = configuration["SLACK_BOT_API_TOKEN"],
				UserId = configuration["SLACK_USER_ID"]
			},
			Work = new SlackConfiguration
			{
				UserApiToken = configuration["SLACK_WORK_USER_API_TOKEN"] ?? configuration["SLACK_USER_API_TOKEN"],
				BotToken = configuration["SLACK_WORK_BOT_API_TOKEN"] ?? configuration["SLACK_BOT_API_TOKEN"],
				UserId = configuration["SLACK_WORK_USER_ID"] ?? configuration["SLACK_USER_ID"]
			}

		});

		services.AddScoped<StartPomodoro>();
		services.AddHttpClient<PomodoroRepository>(client =>
		{
			client.BaseAddress = dbApiConnection.DbUri;
		});
		services.AddHostedService<PomodoroStatusChecker>();
		return services;
	}
}