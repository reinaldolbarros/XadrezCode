using ChessMAUI.Models;
using ChessMAUI.Services;

namespace ChessMAUI.Views;

public partial class RankingPage : ContentPage
{
    private bool _showWeekly = false;

    public RankingPage()
    {
        InitializeComponent();
        BuildTierBar();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var svc = AppState.Current.Ranking;
        svc.RankingUpdated -= Refresh;
        svc.RankingUpdated += Refresh;
        Refresh();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        AppState.Current.Ranking.RankingUpdated -= Refresh;
    }

    // -----------------------------------------------------------------------
    // Abas
    // -----------------------------------------------------------------------
    private void OnGlobalTab(object? sender, EventArgs e)
    {
        _showWeekly = false;
        GlobalTab.BackgroundColor = Color.FromArgb("#1A5276");
        GlobalTab.TextColor       = Colors.White;
        WeeklyTab.BackgroundColor = Color.FromArgb("#0F3460");
        WeeklyTab.TextColor       = Color.FromArgb("#AAAACC");
        Refresh();
    }

    private void OnWeeklyTab(object? sender, EventArgs e)
    {
        _showWeekly = true;
        WeeklyTab.BackgroundColor = Color.FromArgb("#1A5276");
        WeeklyTab.TextColor       = Colors.White;
        GlobalTab.BackgroundColor = Color.FromArgb("#0F3460");
        GlobalTab.TextColor       = Color.FromArgb("#AAAACC");
        Refresh();
    }

    // -----------------------------------------------------------------------
    // Faixas de classificação (legenda)
    // -----------------------------------------------------------------------
    private void BuildTierBar()
    {
        TierBar.Children.Clear();
        int[] thresholds = [0, 500, 1500, 3500, 7000, 15000];
        foreach (var min in thresholds)
        {
            var (icon, name, _, _) = ProfileService.GetTier(min);
            var card = new Frame
            {
                BackgroundColor = Color.FromArgb("#0F3460"),
                BorderColor     = Color.FromArgb("#1A4A80"),
                CornerRadius    = 8, Padding = new Thickness(10, 5), HasShadow = false
            };
            var stack = new VerticalStackLayout { HorizontalOptions = LayoutOptions.Center, Spacing = 1 };
            stack.Add(new Label { Text = icon, FontSize = 18, HorizontalTextAlignment = TextAlignment.Center });
            stack.Add(new Label { Text = name, TextColor = Colors.White, FontSize = 9, HorizontalTextAlignment = TextAlignment.Center });
            stack.Add(new Label { Text = min == 0 ? "0+" : $"{min / 1000}k+", TextColor = Color.FromArgb("#AAAACC"), FontSize = 8, HorizontalTextAlignment = TextAlignment.Center });
            card.Content = stack;
            TierBar.Children.Add(card);
        }
    }

    // -----------------------------------------------------------------------
    // Atualiza lista
    // -----------------------------------------------------------------------
    private void Refresh()
    {
        var profile = AppState.Current.Profile;
        var svc     = AppState.Current.Ranking;
        var entries = _showWeekly ? svc.GetWeekly(profile) : svc.GetGlobal(profile);

        RankList.Children.Clear();
        foreach (var e in entries)
            RankList.Children.Add(BuildRow(e));

        // Rodapé com posição do jogador
        var me = entries.First(e => e.IsHuman);
        MyPosLabel.Text    = me.PositionLabel;
        MyTierLabel.Text   = me.TierIcon;
        MyAvatarLabel.Text = me.Avatar;
        MyNameLabel.Text   = me.Name;
        MyPointsLabel.Text = _showWeekly ? $"{me.WeekPoints} pts" : $"{me.Points} pts";
        MyRatingLabel.Text = me.Rating.ToString();
    }

    private static Grid BuildRow(RankingEntry e)
    {
        var row = new Grid
        {
            ColumnDefinitions = { new(44), new(GridLength.Auto), new(GridLength.Star), new(80), new(60) },
            BackgroundColor   = e.IsHuman ? Color.FromArgb("#1C2A0A") : e.RowColor,
            Padding           = new Thickness(10, 7)
        };

        // Posição
        row.Add(new Label
        {
            Text = e.PositionLabel, FontSize = e.Position <= 3 ? 16 : 13,
            TextColor = e.Position <= 3 ? Colors.White : Color.FromArgb("#AAAACC"),
            HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center
        });

        // Faixa
        var tierLbl = new Label
        {
            Text = e.TierIcon, FontSize = 16,
            Margin = new Thickness(8, 0),
            HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(tierLbl, 1);
        row.Add(tierLbl);

        // Avatar + Nome
        var nameStack = new HorizontalStackLayout { Spacing = 6, VerticalOptions = LayoutOptions.Center };
        nameStack.Add(new Label { Text = e.Avatar, FontSize = 14, VerticalOptions = LayoutOptions.Center });
        nameStack.Add(new VerticalStackLayout
        {
            Children =
            {
                new Label { Text = e.Name, TextColor = e.NameColor, FontSize = 13,
                    FontAttributes = e.IsHuman ? FontAttributes.Bold : FontAttributes.None },
                new Label { Text = e.TierName, TextColor = Color.FromArgb("#888899"), FontSize = 10 }
            }
        });
        Grid.SetColumn(nameStack, 2);
        row.Add(nameStack);

        // Pontos
        var ptsLbl = new Label
        {
            Text = $"{e.Points:N0}", TextColor = Color.FromArgb("#4CAF50"),
            FontSize = 13, FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(ptsLbl, 3);
        row.Add(ptsLbl);

        // ELO
        var eloLbl = new Label
        {
            Text = e.Rating.ToString(), TextColor = Color.FromArgb("#AAAACC"),
            FontSize = 12, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(eloLbl, 4);
        row.Add(eloLbl);

        return row;
    }
}
