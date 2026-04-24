using ChessMAUI.Models;
using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class CareerPage : ContentPage
{
    private CareerService Svc => AppState.Current.Career;

    public CareerPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var state = AppState.Current;
        if (state.IsCareerGame && state.MatchResultReady)
        {
            state.IsCareerGame     = false;
            state.MatchResultReady = false;

            var prog = Svc.Progress;
            if (prog.ActiveTournament != null)
            {
                var result = state.LastMatchHumanWon ? CareerRoundResult.Win
                           : state.LastMatchWasDraw  ? CareerRoundResult.Draw
                           : CareerRoundResult.Loss;
                Svc.RecordRound(prog.ActiveTournament, state.CareerOpponentName, result);
                Svc.Save(prog);
            }
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        var prog = Svc.Progress;

        if (prog.IsCareerCompleted) { ShowCareerCompleted(); return; }
        if (prog.ActiveTournament == null) { ShowWelcome();  return; }

        var t = prog.ActiveTournament;

        TournamentNameLabel.Text = t.LevelName;
        LevelLabel.Text          = $"Nível {(int)t.Level + 1} de 7  ·  {t.LevelName}";
        LevelSubLabel.Text       = t.LevelSubtitle;

        if (!t.IsCompleted)
            ShowInProgress(t);
        else
            ShowResult(t, prog);
    }

    // ── Welcome ───────────────────────────────────────────────────────────────

    private void ShowWelcome()
    {
        TournamentNameLabel.Text = "Modo Carreira";
        RoundInfoLabel.Text      = "Do Torneio Local ao Campeonato Mundial";
        LevelLabel.Text          = "7 níveis · Circuito FIDE";
        LevelSubLabel.Text       = "Suba a pirâmide e torne-se Campeão Mundial";

        WelcomeSection.IsVisible   = true;
        StandingsSection.IsVisible = false;
        CopaSection.IsVisible      = false;
        MundialSection.IsVisible   = false;
        OpponentSection.IsVisible  = false;
        PlayBtn.IsVisible          = false;
        ResultSection.IsVisible    = false;
        NextBtn.IsVisible          = false;
    }

    // ── In Progress ───────────────────────────────────────────────────────────

    private void ShowInProgress(CareerTournamentState t)
    {
        WelcomeSection.IsVisible = false;
        ResultSection.IsVisible  = false;
        NextBtn.IsVisible        = false;

        switch (t.Format)
        {
            case CareerFormat.Swiss:       ShowSwissInProgress(t);       break;
            case CareerFormat.Elimination: ShowCopaInProgress(t);        break;
            case CareerFormat.BestOfN:     ShowMundialInProgress(t);     break;
        }
    }

    private void ShowSwissInProgress(CareerTournamentState t)
    {
        RoundInfoLabel.Text = $"Rodada {t.CurrentRound} de {t.TotalRounds}";

        CopaSection.IsVisible    = false;
        MundialSection.IsVisible = false;
        BuildStandings(t);

        var opp = Svc.GetNextOpponent(t);
        OpponentContextLabel.Text = "Próximo adversário";
        OpponentNameLabel.Text    = opp.Name;
        OpponentDiffLabel.Text    = Svc.DiffLabel(opp.Difficulty);
        OpponentSection.IsVisible = true;

        PlayBtn.Text      = $"Jogar Rodada {t.CurrentRound}";
        PlayBtn.IsVisible = true;
    }

    private void ShowCopaInProgress(CareerTournamentState t)
    {
        RoundInfoLabel.Text = t.CurrentRound switch
        {
            1 => "Fase 1 de 3 — Oitavas",
            2 => "Fase 2 de 3 — Semifinal",
            _ => "Fase 3 de 3 — Final"
        };

        StandingsSection.IsVisible = false;
        MundialSection.IsVisible   = false;
        BuildCopaBracket(t);
        CopaSection.IsVisible = true;

        var opp = Svc.GetNextOpponent(t);
        OpponentContextLabel.Text = t.CurrentRound == 3 ? "Finalista — adversário" : "Adversário desta fase";
        OpponentNameLabel.Text    = opp.Name;
        OpponentDiffLabel.Text    = Svc.DiffLabel(opp.Difficulty);
        OpponentSection.IsVisible = true;

        PlayBtn.Text      = t.CurrentRound == 3 ? "Jogar a Final" : $"Jogar Fase {t.CurrentRound}";
        PlayBtn.IsVisible = true;
    }

    private void ShowMundialInProgress(CareerTournamentState t)
    {
        RoundInfoLabel.Text = $"Partida {t.CurrentRound} · Precisa de {t.WinsNeeded} vitórias";

        StandingsSection.IsVisible = false;
        CopaSection.IsVisible      = false;
        BuildMundialScore(t);
        MundialSection.IsVisible  = true;

        var opp = Svc.GetNextOpponent(t);
        OpponentContextLabel.Text = "Adversário — Campeão Mundial";
        OpponentNameLabel.Text    = opp.Name;
        OpponentDiffLabel.Text    = Svc.DiffLabel(opp.Difficulty);
        OpponentSection.IsVisible = true;

        PlayBtn.Text      = $"Jogar Partida {t.CurrentRound}";
        PlayBtn.IsVisible = true;
    }

    // ── Result ────────────────────────────────────────────────────────────────

    private void ShowResult(CareerTournamentState t, CareerProgress prog)
    {
        WelcomeSection.IsVisible  = false;
        OpponentSection.IsVisible = false;
        PlayBtn.IsVisible         = false;

        // Show format-specific summary alongside result
        StandingsSection.IsVisible = t.Format == CareerFormat.Swiss;
        CopaSection.IsVisible      = t.Format == CareerFormat.Elimination;
        MundialSection.IsVisible   = t.Format == CareerFormat.BestOfN;

        switch (t.Format)
        {
            case CareerFormat.Swiss:       BuildStandings(t);       break;
            case CareerFormat.Elimination: BuildCopaBracket(t);     break;
            case CareerFormat.BestOfN:     BuildMundialScore(t);    break;
        }

        bool advanced = t.Outcome is CareerStageOutcome.Advanced or CareerStageOutcome.AdvancedDirect;

        RoundInfoLabel.Text = advanced ? "Classificado!" : "Eliminado";

        ResultSection.IsVisible = true;

        if (t.Level == CareerLevel.Mundial && advanced)
        {
            ResultIcon.Text           = "🏆";
            ResultTitle.Text          = "Campeão Mundial!";
            ResultDetail.Text         = "Você conquistou o título máximo do xadrez.";
            ResultTitle.TextColor     = Color.FromArgb("#FFD700");
            ResultDestination.Text    = "";
            NextBtn.Text              = "Celebrar Título";
            NextBtn.BackgroundColor   = Color.FromArgb("#4A3800");
            NextBtn.IsVisible         = true;
            return;
        }

        if (advanced)
        {
            ResultIcon.Text       = t.Outcome == CareerStageOutcome.AdvancedDirect ? "⚡" : "✓";
            ResultTitle.Text      = t.Outcome == CareerStageOutcome.AdvancedDirect
                ? "Classificação Direta!"
                : "Classificado!";
            ResultDetail.Text = t.Level switch
            {
                CareerLevel.CopaMundo  => "Você venceu a Copa do Mundo FIDE!",
                CareerLevel.GrandSwiss => "Top 2 do Grand Swiss!",
                CareerLevel.GrandPrix  => "Vencedor do Grand Prix!",
                CareerLevel.Candidatos => "Campeão dos Candidatos — você é o desafiante!",
                _ => "Você avançou de fase!"
            };
            ResultTitle.TextColor = Color.FromArgb("#4CAF50");
        }
        else
        {
            ResultIcon.Text   = "✗";
            ResultTitle.Text  = "Eliminado";
            ResultDetail.Text = t.Level switch
            {
                CareerLevel.CopaMundo  => "Você foi eliminado da Copa do Mundo.",
                CareerLevel.GrandSwiss => "Não classificou entre os Top 2.",
                CareerLevel.GrandPrix  => "Não venceu o Grand Prix.",
                CareerLevel.Candidatos => "Não venceu o Candidatos.",
                CareerLevel.Mundial    => "Você perdeu o match do Mundial.",
                _ => "Você não se classificou."
            };
            ResultTitle.TextColor = Color.FromArgb("#FF5252");
        }

        ResultDestination.Text = (t.Level == CareerLevel.Zonal && !advanced)
            ? (prog.ZonalRetries >= 1 ? "→ Volta ao Torneio Local" : "→ Nova tentativa no Zonal (1 chance restante)")
            : Svc.NextDestinationText(t.Level, t.Outcome);

        string btnText = (t.Level, advanced) switch
        {
            (CareerLevel.Local,      true)  => "Jogar Torneio Zonal",
            (CareerLevel.Local,      false) => "Tentar novamente",
            (CareerLevel.Zonal,      true)  => "Jogar Copa do Mundo FIDE",
            (CareerLevel.Zonal,      false) => prog.ZonalRetries >= 1
                                               ? "Volta ao Torneio Local"
                                               : "Tentar novamente no Zonal",
            (CareerLevel.CopaMundo,  true)  => "Jogar Candidatos",
            (CareerLevel.CopaMundo,  false) => "Jogar Grand Swiss",
            (CareerLevel.GrandSwiss, true)  => "Jogar Candidatos",
            (CareerLevel.GrandSwiss, false) => "Jogar Grand Prix",
            (CareerLevel.GrandPrix,  true)  => "Jogar Candidatos",
            (CareerLevel.GrandPrix,  false) => "Nova Copa do Mundo",
            (CareerLevel.Candidatos, true)  => "Jogar o Mundial!",
            (CareerLevel.Candidatos, false) => "Nova Copa do Mundo",
            (CareerLevel.Mundial,    _)     => "Jogar Candidatos novamente",
            _                               => "Continuar"
        };
        NextBtn.Text             = btnText;
        NextBtn.BackgroundColor  = advanced ? Color.FromArgb("#1A5C1A") : Color.FromArgb("#5C1A1A");
        NextBtn.IsVisible        = true;
    }

    private void ShowCareerCompleted()
    {
        TournamentNameLabel.Text   = "Carreira Concluída";
        RoundInfoLabel.Text        = "🏆 Campeão Mundial de Xadrez";
        LevelLabel.Text            = "Todos os 7 níveis completados";
        LevelSubLabel.Text         = "";
        WelcomeSection.IsVisible   = false;
        StandingsSection.IsVisible = false;
        CopaSection.IsVisible      = false;
        MundialSection.IsVisible   = false;
        OpponentSection.IsVisible  = false;
        PlayBtn.IsVisible          = false;
        ResultSection.IsVisible    = true;
        ResultIcon.Text            = "🏆";
        ResultTitle.Text           = "Campeão Mundial!";
        ResultDetail.Text          = "Você chegou ao topo e se tornou Campeão Mundial de Xadrez.";
        ResultDestination.Text     = "";
        ResultTitle.TextColor      = Color.FromArgb("#FFD700");
        NextBtn.IsVisible          = false;
    }

    // ── Swiss Standings ───────────────────────────────────────────────────────

    private void BuildStandings(CareerTournamentState t)
    {
        StandingsSection.IsVisible = true;

        ZoneLabel.Text = t.AdvancementSpots > 1
            ? $"↑ Top {t.AdvancementSpots} avançam"
            : "↑ Apenas o 1º avança";

        StandingsList.Children.Clear();
        var standings = t.Standings;

        for (int i = 0; i < standings.Count; i++)
        {
            var  p      = standings[i];
            int  pos    = i + 1;
            bool isAdv  = pos <= t.AdvancementSpots;
            bool isLast = i == standings.Count - 1;

            string posColor = pos == 1 ? "#FFD700" : pos == 2 ? "#C0C0D0"
                            : pos == 3 ? "#CD7F32" : "#607890";
            string bgHex    = p.IsHuman ? "#1A2A0A" : "#0D1828";

            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection(
                    new(28), new(GridLength.Star), new(20), new(GridLength.Auto)),
                BackgroundColor = Color.FromArgb(bgHex),
                Padding         = new Thickness(12, 7)
            };

            row.Add(new Label
            {
                Text = $"{pos}º", FontSize = 12,
                TextColor = Color.FromArgb(posColor),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center
            });

            var nameLbl = new Label
            {
                Text           = p.IsHuman ? "Você" : p.Name,
                TextColor      = p.IsHuman ? Color.FromArgb("#4CAF50") : Colors.White,
                FontSize       = 13,
                FontAttributes = p.IsHuman ? FontAttributes.Bold : FontAttributes.None,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(nameLbl, 1);
            row.Add(nameLbl);

            string zoneText  = isAdv && !t.IsCompleted ? "↑" : "";
            var zoneLbl = new Label
            {
                Text = zoneText, TextColor = Color.FromArgb("#4CAF50"),
                FontSize = 11, VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
            Grid.SetColumn(zoneLbl, 2);
            row.Add(zoneLbl);

            var ptsLbl = new Label
            {
                Text = p.Points.ToString("0.0"),
                TextColor = Color.FromArgb(p.IsHuman ? "#4CAF50" : "#607890"),
                FontSize = 12, VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(ptsLbl, 3);
            row.Add(ptsLbl);

            StandingsList.Children.Add(row);

            if (!isLast)
                StandingsList.Children.Add(new BoxView
                {
                    Color = Color.FromArgb("#1A2840"), HeightRequest = 1
                });
        }
    }

    // ── Copa Bracket ──────────────────────────────────────────────────────────

    private void BuildCopaBracket(CareerTournamentState t)
    {
        CopaBracket.Children.Clear();

        string[] phaseLabels = ["Oitavas", "Semifinal", "Final"];
        string[] oppNames    = t.Players
            .Where(p => !p.IsHuman)
            .Select(p => p.Name)
            .ToArray();

        for (int i = 0; i < 3; i++)
        {
            CareerRound? round = t.Rounds.FirstOrDefault(r => r.Number == i + 1);
            bool isCurrent = (i + 1) == t.CurrentRound && !t.IsCompleted;
            bool isFuture  = round == null && !isCurrent;

            string icon  = round?.Result switch
            {
                CareerRoundResult.Win  => "✓",
                CareerRoundResult.Loss or CareerRoundResult.Draw => "✗",
                _ => isCurrent ? "▶" : "○"
            };
            string iconColor = round?.Result switch
            {
                CareerRoundResult.Win  => "#4CAF50",
                CareerRoundResult.Loss or CareerRoundResult.Draw => "#FF5252",
                _ => isCurrent ? "#FFD700" : "#607890"
            };

            string oppName = i < oppNames.Length ? oppNames[i] : "—";
            string rowColor = isCurrent ? "#1A2A0A" : "#0D1828";

            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection(
                    new(22), new(70), new(GridLength.Star), new(GridLength.Auto)),
                BackgroundColor = Color.FromArgb(rowColor),
                Padding = new Thickness(8, 8),
            };

            row.Add(new Label
            {
                Text = icon, TextColor = Color.FromArgb(iconColor),
                FontSize = 14, VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            var phaseLbl = new Label
            {
                Text = phaseLabels[i], TextColor = Color.FromArgb("#607890"),
                FontSize = 11, VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(phaseLbl, 1);
            row.Add(phaseLbl);

            var nameLbl = new Label
            {
                Text = oppName,
                TextColor = isFuture ? Color.FromArgb("#3A4A60")
                          : round?.Result == CareerRoundResult.Win ? Color.FromArgb("#4CAF50")
                          : round?.Result != null ? Color.FromArgb("#FF7070")
                          : Colors.White,
                FontSize = 13, FontAttributes = isCurrent ? FontAttributes.Bold : FontAttributes.None,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(nameLbl, 2);
            row.Add(nameLbl);

            string diffText = round != null ? Svc.DiffLabel(round.Difficulty)
                            : isCurrent && i < oppNames.Length
                                ? Svc.DiffLabel(t.Players.FirstOrDefault(p => p.Name == oppName)?.Difficulty ?? 3)
                                : "";
            var diffLbl = new Label
            {
                Text = diffText, TextColor = Color.FromArgb("#4A6888"),
                FontSize = 11, VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(diffLbl, 3);
            row.Add(diffLbl);

            CopaBracket.Children.Add(row);

            if (i < 2)
                CopaBracket.Children.Add(new BoxView
                {
                    Color = Color.FromArgb("#1A2840"), HeightRequest = 1
                });
        }
    }

    // ── Mundial Score ─────────────────────────────────────────────────────────

    private void BuildMundialScore(CareerTournamentState t)
    {
        var opp = t.Players.FirstOrDefault(p => !p.IsHuman);
        MundialScoreLabel.Text = $"{t.HumanWins} × {t.HumanLosses}";
        MundialSubLabel.Text   = $"Você vs {opp?.Name ?? "Magnus"}  ·  Melhor de {t.WinsNeeded * 2 - 1}";

        MundialHistory.Children.Clear();
        foreach (var r in t.Rounds)
        {
            string icon  = r.Result == CareerRoundResult.Win  ? "✓ Vitória"
                         : r.Result == CareerRoundResult.Loss ? "✗ Derrota"
                         : "= Empate";
            string color = r.Result == CareerRoundResult.Win  ? "#4CAF50"
                         : r.Result == CareerRoundResult.Loss ? "#FF5252"
                         : "#607890";
            MundialHistory.Children.Add(new Label
            {
                Text = $"Partida {r.Number}: {icon}",
                TextColor = Color.FromArgb(color),
                FontSize = 12,
                HorizontalTextAlignment = TextAlignment.Center
            });
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private void OnStartCareerClicked(object? sender, EventArgs e)
    {
        Svc.StartCareer();
        RefreshUI();
    }

    private async void OnPlayRoundClicked(object? sender, EventArgs e)
    {
        var prog = Svc.Progress;
        var t    = prog.ActiveTournament;
        if (t == null) return;

        var opp   = Svc.GetNextOpponent(t);
        var state = AppState.Current;

        state.PendingCareerGame     = true;
        state.IsCareerGame          = true;
        state.MatchResultReady      = false;
        state.PendingTournamentGame = false;
        state.PendingFriendGame     = false;
        state.CareerOpponentName    = opp.Name;
        state.CareerAIDepth         = CareerService.GetAIDepth(opp.Difficulty);
        state.CareerTimeMinutes     = CareerService.GetTimeMinutes(opp.Difficulty);

        await Shell.Current.GoToAsync("GamePage");
    }

    private void OnNextActionClicked(object? sender, EventArgs e)
    {
        var prog = Svc.Progress;
        Svc.ApplyStageResult(prog);
        RefreshUI();
    }

    private async void OnShowFlowClicked(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("CareerFlowPage");
}
