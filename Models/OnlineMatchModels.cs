namespace ChessMAUI.Models;

public enum TimeControlOption { Bullet1 = 1, Blitz3 = 3, Blitz5 = 5, Rapid10 = 10, Rapid15 = 15 }

public enum OnlineMatchPhase { Idle, Searching, Confirmed, Cancelled }

public class OnlineMatchState
{
    public string            MatchId          { get; set; } = "";
    public string            OpponentName     { get; set; } = "";
    public string            OpponentAvatar   { get; set; } = "♟";
    public int               OpponentRating   { get; set; }
    public TimeControlOption MyProposal       { get; set; } = TimeControlOption.Blitz5;
    public TimeControlOption AgreedTime       { get; set; } = TimeControlOption.Blitz5;
    public bool              PlayerIsWhite    { get; set; }
    public OnlineMatchPhase  Phase            { get; set; } = OnlineMatchPhase.Idle;
}
