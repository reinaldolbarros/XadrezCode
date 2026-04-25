using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class RandomMatchPage : ContentPage
{
    private const string PrefKey = "online_time_minutes";
    private const int MinTime    = 1;
    private const int MaxTime    = 20;

    private readonly OnlineMatchService    _svc;
    private CancellationTokenSource?      _countdownCts;
    private int _minutes;

    public RandomMatchPage()
    {
        InitializeComponent();
        _svc = AppState.Current.OnlineMatch;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _svc.MatchReady      += OnMatchReady;
        _svc.SearchCancelled += OnSearchCancelled;

        _svc.Reset();
        ShowSection("selection");

        int saved = Preferences.Default.Get(PrefKey, 5);
        SetTime(Math.Clamp(saved, MinTime, MaxTime));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _svc.MatchReady      -= OnMatchReady;
        _svc.SearchCancelled -= OnSearchCancelled;
        _countdownCts?.Cancel();
    }

    // ── Stepper ──────────────────────────────────────────────────────────────

    private void OnDecrease(object? sender, EventArgs e) => SetTime(_minutes - 1);
    private void OnIncrease(object? sender, EventArgs e) => SetTime(_minutes + 1);

    private void SetTime(int value)
    {
        _minutes = Math.Clamp(value, MinTime, MaxTime);
        Preferences.Default.Set(PrefKey, _minutes);

        TimeValueLabel.Text    = _minutes.ToString();
        TimeCategoryLabel.Text = CategoryName(_minutes);
        TimeCategoryLabel.TextColor = _minutes == 1
            ? Color.FromArgb("#FF8C42")   // Bullet laranja
            : _minutes <= 9
                ? Color.FromArgb("#4A8AC0") // Blitz azul
                : Color.FromArgb("#4CAF50"); // Rápido verde

        BtnDecrease.IsEnabled = _minutes > MinTime;
        BtnIncrease.IsEnabled = _minutes < MaxTime;
    }

    // ── Busca ────────────────────────────────────────────────────────────────

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        ShowSection("searching");
        SearchSubLabel.Text = $"{_minutes} min · {CategoryName(_minutes)}  ·  Qualquer rating";
        await _svc.StartSearchingAsync(_minutes);
    }

    private void OnCancelClicked(object? sender, EventArgs e) => _svc.Cancel();

    // ── Eventos do serviço ───────────────────────────────────────────────────

    private void OnMatchReady()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var s = _svc.State;
            OppNameLabel.Text          = string.IsNullOrEmpty(s.OpponentName) ? "Oponente" : s.OpponentName;
            OppRatingLabel.Text        = $"Rating: {s.OpponentRating}";
            ConfirmedDetailsLabel.Text =
                $"{s.AgreedMinutes} min · {CategoryName(s.AgreedMinutes)}  ·  " +
                (s.PlayerIsWhite ? "Você joga com as Brancas ♙" : "Você joga com as Pretas ♟");
            ShowSection("confirmed");

            _countdownCts?.Cancel();
            _countdownCts = new CancellationTokenSource();

            try
            {
                for (int i = 3; i >= 1; i--)
                {
                    CountdownLabel.Text = $"Iniciando em {i}...";
                    await Task.Delay(1000, _countdownCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                ShowSection("selection");
                return;
            }

            var app = AppState.Current;
            app.PendingOnlineGame   = true;
            app.IsOnlineGame        = true;
            app.OnlineOpponentName  = s.OpponentName;
            app.OnlineTimeMinutes   = s.AgreedMinutes;
            app.OnlinePlayerIsWhite = s.PlayerIsWhite;

            await Shell.Current.GoToAsync("GamePage");
        });
    }

    private void OnCountdownCancelled(object? sender, EventArgs e)
    {
        _countdownCts?.Cancel();
        _svc.Reset();
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

    private static string CategoryName(int min) => min switch
    {
        1      => "Bullet",
        <= 9   => "Blitz",
        _      => "Rápido"
    };
}
