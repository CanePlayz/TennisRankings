namespace TennisRankings;

public class ATPRankingService
{
    public Task<ATPRanking[]> GetRankingsAsync()
    {
        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new ATPRanking
        {
            Name = "Player " + index,
            Country = "Country " + index,
            Points = Random.Shared.Next(1000, 2000)
        }).ToArray());
    }
}
