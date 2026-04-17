using ChessMAUI.Models;

namespace ChessMAUI.Views;

public partial class ExtractPage : ContentPage
{
    private enum Filter { All, Credits, Debits }
    private Filter _filter = Filter.All;

    public ExtractPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BuildFilterBar();
        RefreshList();
    }

    // -----------------------------------------------------------------------
    // Filtros
    // -----------------------------------------------------------------------
    private void BuildFilterBar()
    {
        FilterBar.Children.Clear();

        (string Label, Filter Value)[] options =
        [
            ("Todos",    Filter.All),
            ("Entradas", Filter.Credits),
            ("Saídas",   Filter.Debits),
        ];

        foreach (var (label, value) in options)
        {
            bool active = _filter == value;
            var btn = new Button
            {
                Text            = label,
                FontSize        = 12,
                CornerRadius    = 16,
                Padding         = new Thickness(16, 6),
                Margin          = new Thickness(0, 0, 6, 0),
                BackgroundColor = active ? Color.FromArgb("#0F3460") : Color.FromArgb("#252B45"),
                TextColor       = Colors.White,
                FontAttributes  = active ? FontAttributes.Bold : FontAttributes.None
            };
            var captured = value;
            btn.Clicked += (_, _) => { _filter = captured; BuildFilterBar(); RefreshList(); };
            FilterBar.Children.Add(btn);
        }
    }

    // -----------------------------------------------------------------------
    // Lista
    // -----------------------------------------------------------------------
    private void RefreshList()
    {
        var profile = AppState.Current.Profile;
        var all     = profile.GetTransactions();

        // Totais sempre sobre tudo
        decimal totalIn  = all.Where(t => t.Amount > 0).Sum(t => t.Amount);
        decimal totalOut = all.Where(t => t.Amount < 0).Sum(t => t.Amount);

        BalanceLabel.Text   = $"$ {profile.Balance:N0}";
        TotalInLabel.Text   = $"$ {totalIn:N0}";
        TotalOutLabel.Text  = $"$ {Math.Abs(totalOut):N0}";

        // Aplica filtro
        var filtered = _filter switch
        {
            Filter.Credits => all.Where(t => t.Amount > 0).ToList(),
            Filter.Debits  => all.Where(t => t.Amount < 0).ToList(),
            _              => all
        };

        TransactionList.Children.Clear();

        if (filtered.Count == 0)
        {
            TransactionList.Children.Add(new Label
            {
                Text              = "Nenhuma transação encontrada.",
                TextColor         = Color.FromArgb("#555577"),
                FontSize          = 14,
                HorizontalOptions = LayoutOptions.Center,
                Margin            = new Thickness(0, 40, 0, 0)
            });
            return;
        }

        // Agrupa por data
        var groups = filtered
            .GroupBy(t => t.Date.Date)
            .OrderByDescending(g => g.Key);

        bool alt = false;
        foreach (var group in groups)
        {
            // Cabeçalho do dia
            TransactionList.Children.Add(new Frame
            {
                BackgroundColor = Color.FromArgb("#12172A"),
                BorderColor     = Color.FromArgb("#1E2A4A"),
                CornerRadius    = 0,
                Padding         = new Thickness(16, 6),
                HasShadow       = false,
                Content         = new Label
                {
                    Text      = FormatDate(group.Key),
                    TextColor = Color.FromArgb("#7788AA"),
                    FontSize  = 11,
                    FontAttributes = FontAttributes.Bold
                }
            });

            foreach (var tx in group.OrderByDescending(t => t.Date))
            {
                TransactionList.Children.Add(BuildRow(tx, alt));
                alt = !alt;
            }
        }
    }

    // -----------------------------------------------------------------------
    // Linha de transação
    // -----------------------------------------------------------------------
    private static Grid BuildRow(TransactionEntry tx, bool alt)
    {
        bool isCredit = tx.Amount >= 0;
        var  row      = new Grid
        {
            ColumnDefinitions = { new(GridLength.Auto), new(GridLength.Star), new(GridLength.Auto) },
            BackgroundColor   = alt ? Color.FromArgb("#16213E") : Color.FromArgb("#12192E"),
            Padding           = new Thickness(16, 10)
        };

        // Ícone
        row.Add(new Label
        {
            Text            = string.IsNullOrEmpty(tx.Icon) ? (isCredit ? "💰" : "💸") : tx.Icon,
            FontSize        = 20,
            VerticalOptions = LayoutOptions.Center,
            Margin          = new Thickness(0, 0, 12, 0)
        });

        // Descrição + hora
        var info = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Spacing = 2 };
        info.Add(new Label
        {
            Text      = tx.Description,
            TextColor = Colors.White,
            FontSize  = 13
        });
        info.Add(new Label
        {
            Text      = tx.Date.ToString("HH:mm"),
            TextColor = Color.FromArgb("#666688"),
            FontSize  = 10
        });
        Grid.SetColumn(info, 1);
        row.Add(info);

        // Valor
        var amountLbl = new Label
        {
            Text              = isCredit ? $"+ $ {tx.Amount:N0}" : $"− $ {Math.Abs(tx.Amount):N0}",
            TextColor         = isCredit ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF5252"),
            FontSize          = 14,
            FontAttributes    = FontAttributes.Bold,
            VerticalOptions   = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };
        Grid.SetColumn(amountLbl, 2);
        row.Add(amountLbl);

        return row;
    }

    private static string FormatDate(DateTime date)
    {
        if (date == DateTime.Today)           return "Hoje";
        if (date == DateTime.Today.AddDays(-1)) return "Ontem";
        return date.ToString("dd/MM/yyyy");
    }
}
