using TennisRankings.Models;

namespace TennisRankings.Services;

public class MenSingleOfficialService
{
    private readonly HttpClient _httpClient;

    public MenSingleOfficialService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TennisRankingOfficial>> GetRankingsAsync()
    {
        string activeEnvironment = Environment.GetEnvironmentVariable("ACTIVE_ENVIRONMENT") ?? "blue";
        string apiUrl = $"http://{activeEnvironment}-api:8080/mensingleofficial";

        var result = await _httpClient.GetFromJsonAsync<List<TennisRankingOfficial>>(apiUrl);
        return result ?? new List<TennisRankingOfficial>();
    }
}
