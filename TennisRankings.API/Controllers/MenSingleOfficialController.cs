using Microsoft.AspNetCore.Mvc;
using TennisRankings.API.Models;
using TennisRankings.API.Services;

namespace TennisRankings.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MenSingleOfficialController : ControllerBase
{
    private readonly MenOfficialRankingScraper _rankingScraper;

    public MenSingleOfficialController(MenOfficialRankingScraper rankingScraper)
    {
        _rankingScraper = rankingScraper;
    }

    [HttpGet]
    public async Task<ActionResult<List<TennisRankingOfficial>>> Get()
    {
        var rankings = await _rankingScraper.ScrapeMenOfficalRankingAsync();
        return Ok(rankings);
    }
}
