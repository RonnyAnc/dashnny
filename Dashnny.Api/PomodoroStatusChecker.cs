using Dashnny.Api.Controllers;
using SlackNet;

namespace Dashnny.Api
{
	internal class PomodoroStatusChecker : BackgroundService
	{
		private readonly PomodoroRepository pomodoroRepository;

		public PomodoroStatusChecker(PomodoroRepository pomodoroRepository)
		{
			this.pomodoroRepository = pomodoroRepository;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var activePomodoro = await pomodoroRepository.FindActive();
				if (activePomodoro != null && activePomodoro.EndTime < DateTime.Now)
				{
					activePomodoro.Complete();
					await pomodoroRepository.Update(activePomodoro);
				}
				await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
			}
		}
	}
}