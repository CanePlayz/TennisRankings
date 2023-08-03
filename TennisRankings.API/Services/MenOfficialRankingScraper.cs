using HtmlAgilityPack;
using TennisRankings.API.Models;

namespace TennisRankings.API.Services;

public class MenOfficialRankingScraper
{
    private readonly HttpClient _httpClient;

    public MenOfficialRankingScraper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TennisRankingOfficial>> ScrapeMenOfficalRankingAsync()
    {
        var response = await _httpClient.GetAsync("https://www.atptour.com/en/rankings/singles");

        var doc = new HtmlDocument();
        doc.LoadHtml(await response.Content.ReadAsStringAsync());

        var rankings = new List<TennisRankingOfficial>();

        var rows = doc.DocumentNode.SelectNodes("//table//tbody//tr");

        foreach (var row in rows)
        {
            // Parse rank change
            string rankChangeWebsite = row.SelectSingleNode(".//td[2]").InnerText.Trim();
            string rankChange;
            if (string.IsNullOrEmpty(rankChangeWebsite))
            {
                rankChange = "0";
            }
            else
            {
                rankChange = rankChangeWebsite;
            }

            // Parse points change
            string pointsChangeWebsite = row.SelectSingleNode(".//td[7]").InnerText.Trim();
            string pointsChange;
            if (string.IsNullOrEmpty(pointsChangeWebsite))
            {
                pointsChange = "0";
            }
            else
            {
                pointsChange = pointsChangeWebsite;
            }

            var ranking = new TennisRankingOfficial
            {
                Name = row.SelectSingleNode(".//td[4]").InnerText.Trim(),
                Age = int.Parse(row.SelectSingleNode(".//td[5]").InnerText.Trim()),
                // Country = row.SelectSingleNode(".//td[3]").InnerText.Trim(),
                Points = int.Parse(row.SelectSingleNode(".//td[6]").InnerText.Trim().Replace(",", "")),
                Rank = int.Parse(row.SelectSingleNode(".//td[1]").InnerText.Trim()),
                RankChange = rankChange,
                PointsChange = pointsChange,
                // CurrentTournament =
                // CurrentTournamentRound =
                // NextTournament =
            };
            rankings.Add(ranking);
        }

        return rankings;
    }
}
