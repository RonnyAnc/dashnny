internal class DashnnyConfiguration
{
	public DashnnyConfiguration(string dashnnyApiUrl, string dashnnyApiKey)
	{
		DashnnyApiUrl = dashnnyApiUrl;
		DashnnyApiKey = dashnnyApiKey;
	}

	public string DashnnyApiUrl { get; }
	public string DashnnyApiKey { get; }
}