using System.Text;
using System.Text.Json;

namespace Dashnny.Api.Controllers;

public class PomodoroRepository
{
	private readonly HttpClient httpClient;
	private readonly DBApiConnection dBApiConnection;

	public PomodoroRepository(HttpClient httpClient, DBApiConnection dBApiConnection)
	{
		this.httpClient = httpClient;
		this.dBApiConnection = dBApiConnection;
	}

	public async Task Create(Pomodoro pomodoro)
	{
		using var request = new HttpRequestMessage(HttpMethod.Post, "pomodoros/items");
		request.Headers.TryAddWithoutValidation("X-API-Key", dBApiConnection.ApiKey);
		request.Content = new InsertItemQuery<Pomodoro>().Serialize(pomodoro);
		var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
		if (responseMessage.IsSuccessStatusCode)
		{
			return;
		}
		if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
		{
			throw new UnauthorizedAccessException();
		}
	}

	internal async Task<Pomodoro> FindActive()
	{
		using var request = new HttpRequestMessage(HttpMethod.Post, "pomodoros/query");
		request.Headers.TryAddWithoutValidation("X-API-Key", dBApiConnection.ApiKey);
		var query = "{ \"query\": [{ \"isActive\": true }] }";
		request.Content = new StringContent(query, Encoding.UTF8, "application/json");
		var responseMessage = await httpClient.SendAsync(request);
		return JsonSerializer.Deserialize<QueryResponse<Pomodoro>>(responseMessage.Content.ReadAsStream(), new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true
		}).Items.FirstOrDefault();
	}

	internal async Task Update(Pomodoro pomodoro)
	{
		using var request = new HttpRequestMessage(HttpMethod.Patch, $"pomodoros/items/{pomodoro.Key}");
		request.Headers.TryAddWithoutValidation("X-API-Key", dBApiConnection.ApiKey);
		var pomodoroUpdateSet = new PomodoroUpdateSet
		{
			IsActive = pomodoro.IsActive,
			IsCompleted = pomodoro.IsCompleted
		};
		request.Content = new UpdateItemQuery<PomodoroUpdateSet>().Serialize(pomodoroUpdateSet);
		var responseMessage = await httpClient.SendAsync(request);
		if (!responseMessage.IsSuccessStatusCode)
		{
			throw new Exception(await responseMessage.Content.ReadAsStringAsync());
		}
	}
}

public class PomodoroUpdateSet
{
	public bool IsActive { get; set; }
	public bool IsCompleted { get; set; }
}

public class QueryResponse<T>
{
	public List<T> Items { get; set; }
}

public class InsertItemQuery<T>
{
	public StringContent Serialize(T item)
	{
		string serializedPomodoro = $"{{\"item\": {JsonSerializer.Serialize(item, new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true
		})}}}";
		return new StringContent(serializedPomodoro, Encoding.UTF8, "application/json");
	}
}

public class UpdateItemQuery<T>
{
	public StringContent Serialize(T item)
	{
		string serializedPomodoro = $"{{\"set\": {JsonSerializer.Serialize(item, new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true
		})}}}";
		return new StringContent(serializedPomodoro, Encoding.UTF8, "application/json");
	}
}