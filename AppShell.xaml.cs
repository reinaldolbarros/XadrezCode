using ChessMAUI.Views;

namespace ChessMAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("WaitingRoomPage",       typeof(WaitingRoomPage));
        Routing.RegisterRoute("BracketPage",           typeof(BracketPage));
        Routing.RegisterRoute("GamePage",              typeof(GamePage));
        Routing.RegisterRoute("TournamentLobbyPage",   typeof(TournamentLobbyPage));
        Routing.RegisterRoute("TournamentHistoryPage", typeof(TournamentHistoryPage));
    }
}
