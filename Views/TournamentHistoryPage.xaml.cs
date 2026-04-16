using ChessMAUI.Models;

namespace ChessMAUI.Views;

public partial class TournamentHistoryPage : ContentPage
{
    public TournamentHistoryPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var records = AppState.Current.History.Records;

        // Stats
        decimal totalBuyIn  = records.Sum(r => r.BuyIn);
        decimal totalPrize  = records.Sum(r => r.Prize);
        decimal profit      = totalPrize - totalBuyIn;

        TotalLabel.Text  = records.Count.ToString();
        WonLabel.Text    = records.Count(r => r.Position == 1).ToString();
        ProfitLabel.Text = $"{(profit >= 0 ? "+" : "")}$ {profit:N0}";
        ProfitLabel.TextColor = profit >= 0 ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF5252");

        // Lista
        HistoryList.Children.Clear();
        if (!records.Any())
        {
            HistoryList.Children.Add(new Label
            {
                Text = "Nenhum torneio disputado ainda.",
                TextColor = Color.FromArgb("#AAAACC"),
                FontSize = 14,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 40)
            });
            return;
        }

        foreach (var r in records)
            HistoryList.Children.Add(BuildCard(r));
    }

    private static Frame BuildCard(TournamentRecord r)
    {
        var frame = new Frame
        {
            BackgroundColor = Color.FromArgb("#16213E"),
            BorderColor     = Color.FromArgb("#0F3460"),
            CornerRadius    = 10,
            Padding         = new Thickness(14, 10),
            HasShadow       = false
        };

        var grid = new Grid { ColumnDefinitions = { new(GridLength.Star), new(GridLength.Auto) } };

        var left = new VerticalStackLayout { Spacing = 2 };
        left.Add(new Label { Text = r.Result, TextColor = Colors.White, FontSize = 14, FontAttributes = FontAttributes.Bold });
        left.Add(new Label
        {
            Text      = $"{r.Date:dd/MM/yyyy}  •  {r.Size} jogadores  •  Buy-in: $ {r.BuyIn:N0}",
            TextColor = Color.FromArgb("#AAAACC"), FontSize = 11
        });
        grid.Add(left);

        var right = new Label
        {
            Text              = r.PrizeText,
            TextColor         = r.PrizeColor,
            FontSize          = 16,
            FontAttributes    = FontAttributes.Bold,
            VerticalOptions   = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };
        Grid.SetColumn(right, 1);
        grid.Add(right);

        frame.Content = grid;
        return frame;
    }
}
