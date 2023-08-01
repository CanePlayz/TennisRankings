namespace TennisRankings.API;

public class TennisRankingBase
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public int HighestRank { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Country { get; set; }
    public int Points { get; set; }
    public int Move { get; set; }
    public string CurrentTournament { get; set; }
    public string CurrentTournamentRound { get; set; }
    public string NextTournament { get; set; }
}
