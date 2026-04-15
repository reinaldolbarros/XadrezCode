using ChessMAUI.Models;
using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class BracketPage : ContentPage
{
    public BracketPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var state = AppState.Current;
        if (state.ActiveTournament == null) return;

        if (state.MatchResultReady)
        {
            state.MatchResultReady = false;
            ProcessMatchResult(state.LastMatchHumanWon);
            if (state.ActiveTournament == null) return;
        }

        RefreshUI();
    }

    // -----------------------------------------------------------------------
    // Processa resultado, atualiza ELO, registra histórico, avança torneio
    // -----------------------------------------------------------------------
    private async void ProcessMatchResult(bool humanWon)
    {
        var state = AppState.Current;
        var t     = state.ActiveTournament!;
        var svc   = state.TournSvc;
        var prof  = state.Profile;
        int ratingBefore = prof.Rating;

        svc.RecordHumanResult(t, humanWon);

        // ── Heads-Up: série ainda não decidida → mostra placar e aguarda próxima partida
        if (t.IsHeadsUp && !t.HeadsUpSeriesDecided)
        {
            string scoreMsg = humanWon
                ? $"Você venceu!\nPlacar: {t.HumanSeriesWins}–{t.OpponentSeriesWins}"
                : $"Adversário venceu!\nPlacar: {t.HumanSeriesWins}–{t.OpponentSeriesWins}";
            await DisplayAlert("Heads-Up · Melhor de 3", scoreMsg + "\n\nPróximo jogo!", "Jogar");
            RefreshUI();
            return;
        }

        if (humanWon)
        {
            prof.RecordWin();
            prof.AddPoints(15);
            bool m3Done = state.Daily.RecordTournamentElimination();
            if (m3Done) { var m3 = state.Daily.GetMissions()[2]; prof.Credit(m3.BalanceReward); prof.AddXp(m3.XpReward); }
        }
        else
        {
            prof.RecordLoss();
        }

        // Atualiza ELO e XP
        prof.UpdateElo(humanWon, state.TournamentOpponentRating);
        prof.AddXp(humanWon ? 40 : 15);

        if (!humanWon)
        {
            decimal prize = svc.GetHumanPrize(t);
            if (prize > 0) prof.Credit(prize);

            // Registra histórico
            state.History.Add(new TournamentRecord
            {
                Size = t.Size, BuyIn = t.BuyIn, Prize = prize,
                Position = t.HumanPlayer?.FinalPosition ?? t.Size,
                RatingBefore = ratingBefore, RatingAfter = prof.Rating
            });

            if (prize > 0) prof.AddPoints(25);

            string eloTxt = $"\nRating: {ratingBefore} → {prof.Rating}";
            string msg = t.IsHeadsUp
                ? $"Adversário venceu a série 2–{t.HumanSeriesWins}.\nMelhor sorte da próxima vez!{eloTxt}"
                : prize > 0
                    ? $"Você foi eliminado na {t.RoundName}.\nPrêmio: $ {prize:N0} creditado!{eloTxt}"
                    : $"Você foi eliminado na {t.RoundName}.\nMelhor sorte da próxima vez!{eloTxt}";

            await DisplayAlert("Eliminado!", msg, "OK");
            state.ActiveTournament = null;
            await Shell.Current.GoToAsync("../..");
            return;
        }

        svc.AdvanceRound(t);

        // Exibe banner de bubble / ITM após avançar
        var alert = svc.CheckAlert(t);
        if (alert == TournamentAlert.Bubble)
            ShowBanner("⚠️  VOCÊ ESTÁ NA BOLHA!  Um eliminado e você recebe prêmio.", "#FF9800");
        else if (alert == TournamentAlert.InTheMoney)
            ShowBanner("💰  PARABÉNS! Você está na zona de premiação!", "#4CAF50");
        else
            HideBanner();

        if ((t.IsHeadsUp || svc.CheckCompletion(t)) && t.Status == TournamentStatus.HumanWon)
        {
            prof.TournamentsWon++;

            // Pontos e XP por vencer o torneio (proporcional ao tamanho)
            int tournPts = t.Size switch { 64 => 400, 32 => 200, 16 => 100, 8 => 50, 2 => 20, _ => 50 };
            prof.AddPoints(tournPts);
            int tournXp  = t.Size switch { 64 => 500, 32 => 250, 16 => 120, 8 => 60, 2 => 25, _ => 60 };
            prof.AddXp(tournXp);

            string winMsg;
            decimal prize = 0;

            if (t.Type == TournamentType.Satellite && t.SatelliteTarget > 0)
            {
                // Satélite: prêmio é um ticket para o torneio alvo
                prof.AddTicket(t.SatelliteTarget);
                state.History.Add(new TournamentRecord
                {
                    Size = t.Size, BuyIn = t.BuyIn, Prize = 0, Position = 1,
                    RatingBefore = ratingBefore, RatingAfter = prof.Rating
                });
                winMsg = $"Parabéns! Você ganhou uma VAGA no torneio de $ {t.SatelliteTarget:N0}!\n\n" +
                         $"🎟 Ticket adicionado à sua conta.\nUse-o no torneio correspondente!\n\n" +
                         $"Rating: {ratingBefore} → {prof.Rating}";
                await DisplayAlert("🎟 VAGA CONQUISTADA!", winMsg, "Incrível!");
            }
            else
            {
                prize = svc.GetHumanPrize(t);
                prof.Credit(prize);
                state.History.Add(new TournamentRecord
                {
                    Size = t.Size, BuyIn = t.BuyIn, Prize = prize, Position = 1,
                    RatingBefore = ratingBefore, RatingAfter = prof.Rating
                });
                winMsg = $"Parabéns! Você venceu o torneio de {t.Size} jogadores!\n\n" +
                         $"Prêmio: $ {prize:N0} creditado!\nRating: {ratingBefore} → {prof.Rating}";
                await DisplayAlert("🏆 CAMPEÃO!", winMsg, "Incrível!");
            }

            state.ActiveTournament = null;
            await Shell.Current.GoToAsync("//LobbyPage");
        }
    }

    // -----------------------------------------------------------------------
    // UI
    // -----------------------------------------------------------------------
    private void RefreshUI()
    {
        var t = AppState.Current.ActiveTournament!;
        var svc = AppState.Current.TournSvc;

        RoundLabel.Text   = t.IsHeadsUp ? $"Heads-Up · {t.HeadsUpScore}" : t.RoundName;
        PlayersLabel.Text = t.IsHeadsUp
            ? $"Melhor de 3  ·  Jogo {t.HumanSeriesWins + t.OpponentSeriesWins + 1}"
            : $"{t.PlayersRemaining} jogadores restantes de {t.Size}";
        PoolLabel.Text    = $"$ {t.PrizePool:N0}";
        t.PrizeTable.TryGetValue(1, out decimal top);
        YourPrizeLabel.Text = $"$ {top:N0}";

        // Banner bubble/ITM
        var alert = svc.CheckAlert(t);
        if (alert == TournamentAlert.Bubble)
            ShowBanner("⚠️  BOLHA — próximo eliminado não recebe prêmio!", "#FF9800");
        else if (alert == TournamentAlert.InTheMoney)
            ShowBanner("💰  VOCÊ ESTÁ NA ZONA DE PRÊMIOS!", "#4CAF50");
        else
            HideBanner();

        BuildPrizeRow(t);
        BuildMatchList(t);

        var humanMatch = t.CurrentHumanMatch;
        ActionBtn.IsVisible = humanMatch != null;
        if (humanMatch != null)
        {
            var opp = humanMatch.Opponent(t.HumanPlayer!)!;
            ActionBtn.Text = $"▶  JOGAR  vs  {opp.Avatar} {opp.Name}  ({opp.Rating})";
        }
        else if (!t.CurrentRoundMatches.Any(m => m.Status == MatchStatus.Pending))
        {
            // Rodada ainda não gerada — aguardando simulação dos outros jogos
            ActionBtn.IsVisible = false;
            ShowBanner("⏳  Aguardando próxima rodada...", "#0F3460");
        }
    }

    private void ShowBanner(string text, string hexColor)
    {
        AlertLabel.Text               = text;
        AlertLabel.TextColor          = Color.FromArgb(hexColor);
        AlertBanner.BackgroundColor   = Color.FromArgb(hexColor + "22");
        AlertBanner.BorderColor       = Color.FromArgb(hexColor);
        AlertBanner.IsVisible         = true;
    }

    private void HideBanner() => AlertBanner.IsVisible = false;

    // -----------------------------------------------------------------------
    // Tabela de premiação
    // -----------------------------------------------------------------------
    private void BuildPrizeRow(Tournament t)
    {
        PrizeRow.Children.Clear();
        foreach (var (pos, prize) in t.PrizeTable.OrderBy(p => p.Key))
        {
            string suffix = pos switch { 1 => "º 🥇", 2 => "º 🥈", 3 => "º 🥉", _ => "º" };
            var card = new Frame
            {
                BackgroundColor = pos == 1 ? Color.FromArgb("#FFD700")
                                : pos == 2 ? Color.FromArgb("#C0C0C0")
                                : pos == 3 ? Color.FromArgb("#CD7F32")
                                : Color.FromArgb("#0F3460"),
                CornerRadius = 8, Padding = new Thickness(12, 6), HasShadow = false
            };
            var stack = new VerticalStackLayout { HorizontalOptions = LayoutOptions.Center };
            stack.Add(new Label { Text = $"{pos}{suffix}", FontSize = 11,
                TextColor = pos <= 3 ? Colors.Black : Colors.White,
                HorizontalTextAlignment = TextAlignment.Center });
            stack.Add(new Label { Text = $"$ {prize:N0}", FontSize = 14, FontAttributes = FontAttributes.Bold,
                TextColor = pos <= 3 ? Colors.Black : Color.FromArgb("#FFD700"),
                HorizontalTextAlignment = TextAlignment.Center });
            card.Content = stack;
            PrizeRow.Children.Add(card);
        }
    }

    // -----------------------------------------------------------------------
    // Lista de partidas com ratings visíveis
    // -----------------------------------------------------------------------
    private void BuildMatchList(Tournament t)
    {
        MatchContainer.Children.Clear();
        var humanMatch = t.CurrentRoundMatches.FirstOrDefault(m => m.IsHumanMatch);
        if (humanMatch != null)
            MatchContainer.Children.Add(BuildMatchCard(humanMatch, isHuman: true));
        foreach (var m in t.CurrentRoundMatches.Where(m => !m.IsHumanMatch))
            MatchContainer.Children.Add(BuildMatchCard(m, isHuman: false));
    }

    private static Frame BuildMatchCard(TournamentMatch m, bool isHuman)
    {
        bool completed  = m.Status == MatchStatus.Completed;
        Color borderCol = isHuman   ? Color.FromArgb("#FFD700")
                        : completed ? Color.FromArgb("#333355")
                        : Color.FromArgb("#0F3460");

        var card = new Frame
        {
            BackgroundColor = isHuman ? Color.FromArgb("#1C2240") : Color.FromArgb("#16213E"),
            BorderColor = borderCol, CornerRadius = 10,
            Padding = new Thickness(12, 10), HasShadow = false
        };

        var grid = new Grid { ColumnDefinitions = { new(GridLength.Star), new(GridLength.Auto) } };

        // Jogadores com avatar e rating
        var names = new VerticalStackLayout { Spacing = 4 };
        names.Add(PlayerRow(m.Player1));
        names.Add(new Label { Text = "vs", TextColor = Color.FromArgb("#555577"),
            FontSize = 10, Margin = new Thickness(24, 0) });
        names.Add(PlayerRow(m.Player2));
        grid.Add(names);

        // Status
        string statusText; Color statusColor;
        if (completed)
        {
            bool hw = m.Winner?.IsHuman == true;
            statusText  = hw ? "✓ Vitória" : (isHuman ? "✗ Eliminado" : $"✓ {m.Winner?.Name}");
            statusColor = hw ? Color.FromArgb("#4CAF50") : (isHuman ? Color.FromArgb("#FF5252") : Color.FromArgb("#666688"));
        }
        else
        {
            statusText  = isHuman ? "▶ Jogar" : "⏳";
            statusColor = isHuman ? Color.FromArgb("#FFD700") : Color.FromArgb("#AAAACC");
        }

        var statusLbl = new Label
        {
            Text = statusText, TextColor = statusColor,
            FontSize = 12, FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.End
        };
        Grid.SetColumn(statusLbl, 1);
        grid.Add(statusLbl);
        card.Content = grid;
        return card;
    }

    private static HorizontalStackLayout PlayerRow(TournamentPlayer p)
    {
        var row = new HorizontalStackLayout { Spacing = 6 };
        row.Add(new Label { Text = p.Avatar, FontSize = 14, VerticalOptions = LayoutOptions.Center });
        row.Add(new Label
        {
            Text = $"{p.Name}  ", TextColor = Colors.White,
            FontSize = 13, FontAttributes = p.IsHuman ? FontAttributes.Bold : FontAttributes.None,
            VerticalOptions = LayoutOptions.Center
        });
        row.Add(new Label
        {
            Text = $"({p.Rating})", TextColor = Color.FromArgb("#AAAACC"),
            FontSize = 11, VerticalOptions = LayoutOptions.Center
        });
        return row;
    }

    // -----------------------------------------------------------------------
    // Botão jogar
    // -----------------------------------------------------------------------
    private async void OnActionBtnClicked(object? sender, EventArgs e)
    {
        var state = AppState.Current;
        var t     = state.ActiveTournament!;
        var m     = t.CurrentHumanMatch;
        if (m == null) return;

        var opp = m.Opponent(t.HumanPlayer!)!;
        state.TournamentOpponentName   = opp.Name;
        state.TournamentOpponentRating = opp.Rating;
        state.TournamentAIDepth        = state.TournSvc.GetAIDepth(t);
        state.PendingTournamentGame    = true;
        state.MatchResultReady         = false;

        await Shell.Current.GoToAsync("GamePage");
    }
}
