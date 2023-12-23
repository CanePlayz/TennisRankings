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
            var rankChangeNode = row.SelectSingleNode(".//td[2]");
            var pointsChangeNode = row.SelectSingleNode(".//td[7]");

            // Check if the necessary nodes exist
            if (rankChangeNode == null || pointsChangeNode == null)
            {
                continue; // Skip to the next iteration of the loop if nodes are missing
            }

            // Parse rank change
            string rankChangeWebsite = rankChangeNode.InnerText.Trim();
            string rankChange = string.IsNullOrEmpty(rankChangeWebsite) ? "0" : rankChangeWebsite;

            // Parse points change
            string pointsChangeWebsite = pointsChangeNode.InnerText.Trim();
            string pointsChange = string.IsNullOrEmpty(pointsChangeWebsite) ? "0" : pointsChangeWebsite;

            var ranking = new TennisRankingOfficial
            {
                Name = row.SelectSingleNode(".//td[4]").InnerText.Trim(),
                Age = TryParseInt(row.SelectSingleNode(".//td[5]").InnerText.Trim()),
                // Country = row.SelectSingleNode(".//td[3]").InnerText.Trim(),
                Points = TryParseInt(row.SelectSingleNode(".//td[6]").InnerText.Trim().Replace(",", "")),
                Rank = TryParseInt(row.SelectSingleNode(".//td[1]").InnerText.Trim()),
                RankChange = rankChange,
                PointsChange = pointsChange,
                // CurrentTournament =
                // CurrentTournamentRound =
                // NextTournament =
            };
            rankings.Add(ranking);

            static int TryParseInt(string input)
            {
                if (int.TryParse(input, out int result))
                {
                    return result;
                }
                return 0; // or any default value you consider appropriate
            }
        }

        return rankings;
    }

}
