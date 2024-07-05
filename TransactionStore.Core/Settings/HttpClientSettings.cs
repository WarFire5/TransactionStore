namespace TransactionStore.Core.Settings;

public class HttpClientSettings
{
    public string BaseUrl { get; set; } = "https://194.87.210.5:13000/api/configuration?service=2";
    public int TimeoutSeconds { get; set; } = 380;
}
