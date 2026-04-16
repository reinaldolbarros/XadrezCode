using ChessMAUI.Models;

namespace ChessMAUI.Services;

/// <summary>
/// Gerencia o ranking global e semanal.
/// Bots simulados dão sensação de comunidade ativa.
/// </summary>
public class RankingService
{
    private static readonly (string Avatar, string Name, int Points)[] BotSeeds =
    [
        ("🦁","Magnus AI",    18500), ("🐉","Kasparov Bot",  15200),
        ("👑","Fischer AI",   12800), ("⚡","Tal Bot",        10400),
        ("🎯","Karpov AI",    9100),  ("🔥","Anand Bot",      7800),
        ("💎","Carlsen X",    6500),  ("🌟","Capablanca AI",  5200),
        ("🎭","Morphy Bot",   4100),  ("🛡️","Polgar AI",      3500),
        ("♛","Lasker Bot",   2800),  ("♜","Alekhine AI",    2100),
        ("♝","Petrosian X",  1600),  ("♞","Spassky Bot",    1200),
        ("🦊","Bronstein AI",  900),  ("🐺","Smyslov Bot",    700),
        ("🦅","Euwe AI",       500),  ("🐯","Botvinnik X",    350),
        ("🌊","Lputian Bot",   200),  ("🏔","Morozevich X",   100),
    ];

    private readonly List<RankingEntry> _bots;
    private readonly Random             _rng = Random.Shared;

    public event Action? RankingUpdated;

    public RankingService()
    {
        _bots = BotSeeds.Select(b =>
        {
            var (icon, name) = (b.Avatar, b.Name);
            var tier = ProfileService.GetTier(b.Points);
            return new RankingEntry
            {
                Avatar     = icon,
                Name       = name,
                Points     = b.Points,
                WeekPoints = _rng.Next(0, b.Points / 10),
                TierIcon   = tier.Icon,
                TierName   = tier.Name,
                IsHuman    = false
            };
        }).ToList();

        _ = SimulateActivityAsync();
    }

    // -----------------------------------------------------------------------
    // Ranking global — bots + humano, ordenado por pontos
    // -----------------------------------------------------------------------
    public List<RankingEntry> GetGlobal(ProfileService profile)
    {
        var human = HumanEntry(profile);
        var all   = _bots.Concat([human])
                         .OrderByDescending(e => e.Points)
                         .ToList();

        for (int i = 0; i < all.Count; i++)
            all[i].Position = i + 1;

        return all;
    }

    // -----------------------------------------------------------------------
    // Ranking semanal — ordenado por pontos da semana
    // -----------------------------------------------------------------------
    public List<RankingEntry> GetWeekly(ProfileService profile)
    {
        var human = HumanEntry(profile);
        var all   = _bots.Concat([human])
                         .OrderByDescending(e => e.WeekPoints)
                         .ToList();

        for (int i = 0; i < all.Count; i++)
            all[i].Position = i + 1;

        return all;
    }

    private static RankingEntry HumanEntry(ProfileService p)
    {
        var tier = ProfileService.GetTier(p.Points);
        return new RankingEntry
        {
            Avatar     = p.Avatar,
            Name       = p.Name,
            Points     = p.Points,
            WeekPoints = p.WeekPoints,
            TierIcon   = tier.Icon,
            TierName   = tier.Name,
            IsHuman    = true
        };
    }

    // -----------------------------------------------------------------------
    // Simula pequenas variações de pontos nos bots ao longo do tempo
    // -----------------------------------------------------------------------
    private async Task SimulateActivityAsync()
    {
        while (true)
        {
            await Task.Delay(_rng.Next(8000, 20000));
            var bot = _bots[_rng.Next(_bots.Count)];
            int delta = _rng.Next(5, 50);
            bot.Points     += delta;
            bot.WeekPoints += delta;
            var tier = ProfileService.GetTier(bot.Points);
            bot.TierIcon = tier.Icon;
            bot.TierName = tier.Name;
            MainThread.BeginInvokeOnMainThread(() => RankingUpdated?.Invoke());
        }
    }
}
