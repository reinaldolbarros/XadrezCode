namespace ChessMAUI.Models;

public enum RoomStatus { Open, Starting, InProgress }

public class TournamentRoom
{
    public string       Id          { get; init; } = Guid.NewGuid().ToString()[..6].ToUpper();
    public int          Size        { get; init; }
    public decimal      BuyIn       { get; init; }
    public int          TimeMinutes { get; init; }
    public int          Joined      { get; set; } = 0;
    public RoomStatus   Status      { get; set; } = RoomStatus.Open;

    public decimal PrizePool    => BuyIn * Size;
    public string  SizeLabel    => $"{Size} jogadores";
    public string  StatusLabel  => Status switch
    {
        RoomStatus.Open      => $"{Joined}/{Size} na sala",
        RoomStatus.Starting  => "Iniciando...",
        RoomStatus.InProgress=> "Em andamento",
        _ => ""
    };
    public string TimeLabel => TimeMinutes > 0 ? $"{TimeMinutes} min" : "Livre";
    public Color  StatusColor => Status switch
    {
        RoomStatus.Open      => Color.FromArgb("#4CAF50"),
        RoomStatus.Starting  => Color.FromArgb("#FFD700"),
        RoomStatus.InProgress=> Color.FromArgb("#888888"),
        _ => Colors.White
    };
    public bool CanJoin => Status == RoomStatus.Open && Joined < Size;
}
