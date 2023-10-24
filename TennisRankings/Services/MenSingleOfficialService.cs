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
        string apiUrl;

        if (activeEnvironment == "blue")
        {
            apiUrl = "http://blue-api/mensingleofficial";
        }
        else if (activeEnvironment == "green")
        {
            apiUrl = "http://green-api/mensingleofficial";
        }
        else
        {
            throw new Exception("Invalid environment configuration.");
        }

        var result = await _httpClient.GetFromJsonAsync<List<TennisRankingOfficial>>(apiUrl);
        return result ?? new List<TennisRankingOfficial>();
    }
}
