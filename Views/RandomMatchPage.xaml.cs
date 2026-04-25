using ChessMAUI.Models;
using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class RandomMatchPage : ContentPage
{
    private readonly OnlineMatchService _svc;
    private TimeControlOption _selectedTime = TimeControlOption.Blitz5;

    public RandomMatchPage()
    {
        InitializeComponent();
        _svc = AppState.Current.OnlineMatch;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _svc.MatchReady     += OnMatchReady;
        _svc.SearchCancelled += OnSearchCancelled;

        _svc.Reset();
        ShowSection("selection");
        SelectTime(TimeControlOption.Blitz5);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _svc.MatchReady      -= OnMatchReady;
        _svc.SearchCancelled -= OnSearchCancelled;
    }

    // ── Seleção de controle de tempo ─────────────────────────────────────────

    private void OnTimeSelected(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string s && int.TryParse(s, out int min))
            SelectTime((TimeControlOption)min);
    }

    private void SelectTime(TimeControlOption tc)
    {
        _selectedTime = tc;
        var cards = new (Border border, TimeControlOption opt)[]
        {
            (TcBullet,  TimeControlOption.Bullet1),
            (TcBlitz3,  TimeControlOption.Blitz3),
            (TcBlitz5,  TimeControlOption.Blitz5),
            (TcRapid10, TimeControlOption.Rapid10),
            (TcRapid15, TimeControlOption.Rapid15)
        };
        foreach (var (border, opt) in cards)
        {
            bool sel = opt == tc;
            border.BackgroundColor = sel ? Color.FromArgb("#0A2040") : Color.FromArgb("#0E1828");
            border.Stroke          = new SolidColorBrush(sel
                ? Color.FromArgb("#4A8AC0") : Color.FromArgb("#1A2840"));
        }
    }

    // ── Busca ────────────────────────────────────────────────────────────────

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        ShowSection("searching");
        SearchSubLabel.Text = $"{TimeLabel(_selectedTime)}  ·  Qualquer rating";
        await _svc.StartSearchingAsync(_selectedTime);
    }

    private void OnCancelClicked(object? sender, EventArgs e) => _svc.Cancel();

    // ── Eventos do serviço ───────────────────────────────────────────────────

    private void OnMatchReady()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var s = _svc.State;
            OppNameLabel.Text          = s.OpponentName;
            OppRatingLabel.Text        = $"Rating: {s.OpponentRating}";
            ConfirmedDetailsLabel.Text =
                $"{TimeLabel(s.AgreedTime)}  ·  " +
                (s.PlayerIsWhite ? "Você joga com as Brancas ♙" : "Você joga com as Pretas ♟");
            ShowSection("confirmed");

            for (int i = 3; i >= 1; i--)
            {
                CountdownLabel.Text = $"Iniciando em {i}...";
                await Task.Delay(1000);
            }

            var app = AppState.Current;
            app.PendingOnlineGame   = true;
            app.IsOnlineGame        = true;
            app.OnlineOpponentName  = s.OpponentName;
            app.OnlineTimeMinutes   = (int)s.AgreedTime;
            app.OnlinePlayerIsWhite = s.PlayerIsWhite;

            await Shell.Current.GoToAsync("GamePage");
        });
    }

    private void OnSearchCancelled()
    {
        MainThread.BeginInvokeOnMainThread(() => ShowSection("selection"));
    }

    // ── Auxiliares ───────────────────────────────────────────────────────────

    private void ShowSection(string section)
    {
        SelectionSection.IsVisible  = section == "selection";
        SearchingSection.IsVisible  = section == "searching";
        ConfirmedSection.IsVisible  = section == "confirmed";
        if (section == "searching") SearchSpinner.IsRunning = true;
    }

    private static string TimeLabel(TimeControlOption tc) =>
        $"{(int)tc}' {TypeName(tc)}";

    private static string TypeName(TimeControlOption tc) => tc switch
    {
        TimeControlOption.Bullet1  => "Bullet",
        TimeControlOption.Blitz3   => "Blitz",
        TimeControlOption.Blitz5   => "Blitz",
        TimeControlOption.Rapid10  => "Rápido",
        TimeControlOption.Rapid15  => "Rápido",
        _                          => ""
    };
}
