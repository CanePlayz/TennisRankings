using TennisRankings.API.Services;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHttpClient<MenOfficialRankingScraper>();

var app = builder.Build();
app.MapControllers();
app.Run();
