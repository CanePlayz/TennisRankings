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
        var result = await _httpClient.GetFromJsonAsync<List<TennisRankingOfficial>>("http://api/mensingleofficial");
        return result ?? new List<TennisRankingOfficial>();
    }
}
