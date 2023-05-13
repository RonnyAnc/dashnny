public class DBApiConnection
{
	public DBApiConnection(Uri dbUri, string apiKey)
	{
		ApiKey = apiKey;
		DbUri = dbUri;
	}

	public string ApiKey { get; }
	public Uri DbUri { get; }
}