using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChessMAUI.Models;
using ChessMAUI.Services;

namespace ChessMAUI.ViewModels;

// ============================================================
// SquareViewModel — representa cada casa do tabuleiro
// ============================================================
public class SquareViewModel : INotifyPropertyChanged
{
    private string _pieceSymbol = "";
    private bool   _isLight;
    private bool   _isSelected;
    private bool   _isValidMove;
    private bool   _isLastMove;
    private bool   _isInCheck;

    public int Row { get; }
    public int Col { get; }

    public string PieceSymbol
    {
        get => _pieceSymbol;
        set { _pieceSymbol = value; OnPC(); }
    }
    public bool IsLight
    {
        get => _isLight;
        set { _isLight = value; OnPC(); OnPC(nameof(BackgroundColor)); }
    }
    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPC(); OnPC(nameof(BackgroundColor)); }
    }
    public bool IsValidMove
    {
        get => _isValidMove;
        set { _isValidMove = value; OnPC(); OnPC(nameof(BackgroundColor)); }
    }
    public bool IsLastMove
    {
        get => _isLastMove;
        set { _isLastMove = value; OnPC(); OnPC(nameof(BackgroundColor)); }
    }
    public bool IsInCheck
    {
        get => _isInCheck;
        set { _isInCheck = value; OnPC(); OnPC(nameof(BackgroundColor)); }
    }

    public Color BackgroundColor => (IsSelected, IsInCheck, IsLastMove, IsValidMove, IsLight) switch
    {
        (true, _, _, _, _)     => Color.FromArgb("#F6F669"),
        (_, true, _, _, _)     => Color.FromArgb("#FF4444"),
        (_, _, true, _, _)     => Color.FromArgb("#CDD16E"),
        (_, _, _, true, true)  => Color.FromArgb("#A8E0A8"),
        (_, _, _, true, false) => Color.FromArgb("#5DA05D"),
        (_, _, _, _, true)     => Color.FromArgb("#F0D9B5"),
        _                      => Color.FromArgb("#B58863")
    };

    public SquareViewModel(int row, int col)
    {
        Row = row; Col = col;
        IsLight = (row + col) % 2 == 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPC([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}

// ============================================================
// GameViewModel — lógica principal do jogo
// ============================================================
public class GameViewModel : INotifyPropertyChanged
{
    // --- Estado do jogo ---
    private ChessBoard        _board = new();
    private SquareViewModel?  _selectedSquare;
    private List<ChessMove>   _validMoves   = [];
    private ChessMove?        _lastMove;
    private string            _statusMessage = "Toque em 'Novo Jogo' para começar";
    private bool              _gameOver      = true;
    private bool              _isAIThinking;
    private bool              _awaitingPromotion;
    private ChessMove?        _pendingPromotion;

    // --- Temporizador ---
    private IDispatcherTimer? _clock;
    private TimeSpan          _whiteTime;
    private TimeSpan          _blackTime;
    private bool              _timerEnabled;
    private TimeSpan          _moveTime;
    private TimeSpan          _moveTimeLimit;   // limite configurado conforme duração do jogo
    private bool              _moveTimerActive; // false quando jogo é de 1 min

    // --- Serviços ---
    private AIService                _ai    = new();
    private readonly SoundService    _sound = new();
    private readonly BotChatService  _chat  = new();
    private CancellationTokenSource? _aiCts;

    // ----------------------------------------------------------------
    // Tabuleiro visual
    // ----------------------------------------------------------------
    public SquareViewModel[,] Squares   { get; } = new SquareViewModel[8, 8];
    public List<SquareViewModel> SquareList { get; } = [];

    // ----------------------------------------------------------------
    // Propriedades vinculadas ao XAML
    // ----------------------------------------------------------------
    public string StatusMessage
    {
        get => _statusMessage;
        private set { _statusMessage = value; OnPC(); }
    }

    public bool GameOver
    {
        get => _gameOver;
        private set { _gameOver = value; OnPC(); OnPC(nameof(ShowReturnButton)); OnPC(nameof(ShowMoveTimer)); OnPC(nameof(ShowResignButton)); OnPC(nameof(CanOfferDraw)); }
    }

    public bool IsAIThinking
    {
        get => _isAIThinking;
        private set { _isAIThinking = value; OnPC(); }
    }

    public bool AwaitingPromotion
    {
        get => _awaitingPromotion;
        private set { _awaitingPromotion = value; OnPC(); }
    }

    // Temporizador total
    public string  WhiteTimeDisplay => _timerEnabled ? FormatTime(_whiteTime) : "--:--";
    public string  BlackTimeDisplay => _timerEnabled ? FormatTime(_blackTime) : "--:--";
    public bool    TimerVisible     => _timerEnabled;
    public bool    IsWhiteLowTime   => _timerEnabled && _whiteTime.TotalSeconds < 30 && _whiteTime.TotalSeconds > 0;
    public bool    IsBlackLowTime   => _timerEnabled && _blackTime.TotalSeconds < 30 && _blackTime.TotalSeconds > 0;

    // Temporizador por jogada
    public string  MoveTimeDisplay  => FormatTime(_moveTime);
    public bool    IsMoveTimeLow    => _moveTime.TotalSeconds < (_moveTimeLimit.TotalSeconds * 0.25);
    public bool    ShowMoveTimer    => _moveTimerActive;

    // Modo torneio
    public bool   IsTournamentMode    { get; private set; }
    public string TournamentOpponent  { get; private set; } = "";
    public bool   ShowNewGameButton   => !IsTournamentMode;
    public bool   ShowReturnButton    => IsTournamentMode && _gameOver;
    public bool?  HumanWon            { get; private set; } // resultado final em modo torneio

    // Som
    public bool SoundEnabled
    {
        get => _sound.Enabled;
        set { _sound.Enabled = value; OnPC(); }
    }

    // ----------------------------------------------------------------
    // Comandos
    // ----------------------------------------------------------------
    public ICommand SquareTappedCommand { get; }
    public ICommand PromoteCommand      { get; }
    public ICommand ResignCommand       { get; }
    public ICommand OfferDrawCommand    { get; }

    public bool ShowResignButton => !_gameOver;
    public bool CanOfferDraw    => !IsTournamentMode && !_gameOver && !_isAIThinking;

    public event Action<string>?       PromotionRequested;
    public event Action<string>?       ChatMessageReceived;
    public event Action<bool>?         TournamentGameEnded;
    public event Func<Task<bool>>?     ResignRequested;     // retorna true se confirmado
    public event Func<Task<bool>>?     DrawOfferRequested;  // retorna true se aceito pela IA

    // ----------------------------------------------------------------
    // Construtor
    // ----------------------------------------------------------------
    public GameViewModel()
    {
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                var sq = new SquareViewModel(r, c);
                Squares[r, c] = sq;
                SquareList.Add(sq);
            }

        SquareTappedCommand = new Command<SquareViewModel>(OnSquareTapped);
        PromoteCommand      = new Command<string>(OnPromote);
        ResignCommand       = new Command(async () => await OnResign());
        OfferDrawCommand    = new Command(async () => await OnOfferDraw());
        _chat.MessageReceived += msg => ChatMessageReceived?.Invoke(msg);

        RefreshBoard();
    }

    // ----------------------------------------------------------------
    // ----------------------------------------------------------------
    // Modo torneio — chamado pela GamePage quando IsInTournamentMatch
    // ----------------------------------------------------------------
    public void StartTournamentGame(string opponentName, int minutes, int aiDepth)
    {
        _ai                = new AIService(aiDepth);
        TournamentOpponent = opponentName;
        StartNewGame(minutes, isTournament: true);
    }

    // ----------------------------------------------------------------
    // Novo jogo — chamado pela GamePage após o usuário escolher o tempo
    // ----------------------------------------------------------------
    public void StartNewGame(int minutes, bool isTournament = false)
    {
        _aiCts?.Cancel();
        _aiCts = null;

        _board            = new ChessBoard();
        _selectedSquare   = null;
        _lastMove         = null;
        _pendingPromotion = null;
        _validMoves.Clear();
        AwaitingPromotion = false;
        IsAIThinking      = false;
        GameOver          = false;
        HumanWon          = null;
        IsTournamentMode  = isTournament;
        OnPC(nameof(IsTournamentMode));
        OnPC(nameof(TournamentOpponent));
        OnPC(nameof(ShowNewGameButton));
        OnPC(nameof(ShowReturnButton));

        // Configura temporizador
        _timerEnabled = minutes > 0;
        _whiteTime    = TimeSpan.FromMinutes(minutes);
        _blackTime    = TimeSpan.FromMinutes(minutes);
        NotifyTimerProperties();

        // Define limite por jogada conforme duração total:
        // 0 (sem limite) ou 1 min → sem contador por jogada
        // 2 min → 30 s por jogada
        // 3+ min → 2 min por jogada
        _moveTimerActive = minutes >= 2;
        _moveTimeLimit   = minutes == 2 ? TimeSpan.FromSeconds(30) : TimeSpan.FromMinutes(2);
        _moveTime        = _moveTimeLimit;
        OnPC(nameof(MoveTimeDisplay));
        OnPC(nameof(IsMoveTimeLow));
        OnPC(nameof(ShowMoveTimer));

        ClearHighlights();
        RefreshBoard();

        // Clock roda se há timer total OU contador por jogada
        if (_timerEnabled || _moveTimerActive)
            StartClock();
        else
            StopClock();

        StatusMessage = _timerEnabled
            ? $"Brancas jogam — {minutes} min por lado"
            : "Vez das Brancas";

        if (IsTournamentMode) _chat.SendStart();
    }

    // ----------------------------------------------------------------
    // Relógio
    // ----------------------------------------------------------------
    private void StartClock()
    {
        StopClock();
        _clock          = Application.Current!.Dispatcher.CreateTimer();
        _clock.Interval = TimeSpan.FromSeconds(1);
        _clock.Tick    += OnClockTick;
        _clock.Start();
    }

    private void StopClock() { _clock?.Stop(); _clock = null; }

    private void OnClockTick(object? sender, EventArgs e)
    {
        if (_gameOver) return;

        // Contador por jogada — só corre na vez do jogador (brancas)
        if (_moveTimerActive && _board.CurrentTurn == PieceColor.White)
        {
            _moveTime -= TimeSpan.FromSeconds(1);
            OnPC(nameof(MoveTimeDisplay));
            OnPC(nameof(IsMoveTimeLow));
            if (_moveTime <= TimeSpan.Zero)
            {
                _moveTime = TimeSpan.Zero;
                OnPC(nameof(MoveTimeDisplay));
                EndByMoveTimeout();
                return;
            }
        }

        if (!_timerEnabled) return;

        // Contador total de jogo
        if (_board.CurrentTurn == PieceColor.White)
        {
            _whiteTime -= TimeSpan.FromSeconds(1);
            if (_whiteTime <= TimeSpan.Zero)
            {
                _whiteTime = TimeSpan.Zero;
                NotifyTimerProperties();
                EndByTimeout("Tempo esgotado! Pretas (IA) vencem!");
                return;
            }
            OnPC(nameof(WhiteTimeDisplay));
            OnPC(nameof(IsWhiteLowTime));
        }
        else
        {
            _blackTime -= TimeSpan.FromSeconds(1);
            if (_blackTime <= TimeSpan.Zero)
            {
                _blackTime = TimeSpan.Zero;
                NotifyTimerProperties();
                EndByTimeout("Tempo esgotado! Brancas vencem!");
                return;
            }
            OnPC(nameof(BlackTimeDisplay));
            OnPC(nameof(IsBlackLowTime));
        }
    }

    private void EndByMoveTimeout()
    {
        StopClock();
        StatusMessage = "Tempo por jogada esgotado! Pretas (IA) vencem!";
        if (IsTournamentMode) SetTournamentResult(false);
        GameOver = true;
        _sound.PlayGameOver();
    }

    private void EndByTimeout(string msg)
    {
        StopClock();
        StatusMessage = msg;
        if (IsTournamentMode) SetTournamentResult(msg.Contains("Brancas vencem"));
        GameOver = true;
        _sound.PlayGameOver();
    }

    private void ResetMoveTimer()
    {
        _moveTime = _moveTimeLimit;
        OnPC(nameof(MoveTimeDisplay));
        OnPC(nameof(IsMoveTimeLow));
    }

    // Registra resultado do torneio na AppState imediatamente (independente de qual botão o usuário clicar)
    private void SetTournamentResult(bool humanWon)
    {
        HumanWon = humanWon;
        AppState.Current.LastMatchHumanWon = humanWon;
        TournamentGameEnded?.Invoke(humanWon);
    }

    private void NotifyTimerProperties()
    {
        OnPC(nameof(WhiteTimeDisplay));
        OnPC(nameof(BlackTimeDisplay));
        OnPC(nameof(TimerVisible));
        OnPC(nameof(IsWhiteLowTime));
        OnPC(nameof(IsBlackLowTime));
    }

    // ----------------------------------------------------------------
    // Interação com o tabuleiro (somente turno das Brancas)
    // ----------------------------------------------------------------
    private void OnSquareTapped(SquareViewModel tapped)
    {
        if (_gameOver || _awaitingPromotion || _isAIThinking) return;
        if (_board.CurrentTurn != PieceColor.White) return;

        var piece = _board.GetPiece(tapped.Row, tapped.Col);

        if (_selectedSquare != null)
        {
            var move = _validMoves.FirstOrDefault(m => m.ToRow == tapped.Row && m.ToCol == tapped.Col);

            if (move != null)
            {
                if (move.PromotionPiece.HasValue)
                {
                    _pendingPromotion = move;
                    AwaitingPromotion = true;
                    ClearHighlights();
                    PromotionRequested?.Invoke("white");
                    return;
                }
                ExecutePlayerMove(move);
                return;
            }

            if (piece?.Color == PieceColor.White)
            {
                SelectSquare(tapped);
                return;
            }

            ClearHighlights();
            _selectedSquare = null;
            return;
        }

        if (piece?.Color == PieceColor.White)
            SelectSquare(tapped);
    }

    private void SelectSquare(SquareViewModel sq)
    {
        ClearHighlights();
        _selectedSquare = sq;
        _validMoves     = ChessEngine.GetLegalMoves(_board, sq.Row, sq.Col);
        sq.IsSelected   = true;
        foreach (var m in _validMoves)
            Squares[m.ToRow, m.ToCol].IsValidMove = true;
    }

    private void ExecutePlayerMove(ChessMove move)
    {
        bool isCapture = _board.GetPiece(move.ToRow, move.ToCol) != null || move.IsEnPassant;
        _lastMove       = move;
        ClearHighlights();
        _selectedSquare = null;
        _validMoves.Clear();

        ChessEngine.ApplyMove(_board, move);
        RefreshBoard();

        var state = ChessEngine.GetGameState(_board);
        PlaySound(isCapture, state);
        if (IsTournamentMode) _chat.SendGoodMove();
        UpdateStatus(state);

        if (!_gameOver)
            _ = RunAIAsync();
        else
            ResetMoveTimer(); // esconde o contador quando o jogo acaba
    }

    private void OnPromote(string pieceType)
    {
        if (_pendingPromotion == null) return;

        _pendingPromotion.PromotionPiece = pieceType switch
        {
            "queen"  => PieceType.Queen,
            "rook"   => PieceType.Rook,
            "bishop" => PieceType.Bishop,
            "knight" => PieceType.Knight,
            _        => PieceType.Queen
        };

        AwaitingPromotion = false;
        ExecutePlayerMove(_pendingPromotion);
        _pendingPromotion = null;
    }

    // ----------------------------------------------------------------
    // IA (Pretas)
    // ----------------------------------------------------------------
    private async Task RunAIAsync()
    {
        _aiCts?.Cancel();
        _aiCts       = new CancellationTokenSource();
        IsAIThinking = true;

        try
        {
            var move = await _ai.GetBestMoveAsync(_board, _aiCts.Token);
            if (move == null || _gameOver || _board.CurrentTurn != PieceColor.Black) return;

            bool isCapture = _board.GetPiece(move.ToRow, move.ToCol) != null || move.IsEnPassant;
            _lastMove = move;

            ChessEngine.ApplyMove(_board, move);
            RefreshBoard();

            var state = ChessEngine.GetGameState(_board);
            PlaySound(isCapture, state);
            if (IsTournamentMode)
            {
                if (isCapture) _chat.SendCapture();
                if (state == GameState.Check) _chat.SendCheck();
                if (state is GameState.Checkmate or GameState.Stalemate) _chat.SendWin();
            }
            UpdateStatus(state);
            ResetMoveTimer(); // IA jogou — reinicia o contador do jogador
        }
        catch (OperationCanceledException) { }
        finally
        {
            IsAIThinking = false;
        }
    }

    // ----------------------------------------------------------------
    // Som
    // ----------------------------------------------------------------
    private void PlaySound(bool isCapture, GameState state)
    {
        if (state is GameState.Checkmate or GameState.Stalemate or GameState.Draw)
            _sound.PlayGameOver();
        else if (state == GameState.Check)
            _sound.PlayCheck();
        else if (isCapture)
            _sound.PlayCapture();
        else
            _sound.PlayMove();
    }

    // ----------------------------------------------------------------
    // Utilitários
    // ----------------------------------------------------------------
    private void ClearHighlights()
    {
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                Squares[r, c].IsSelected  = false;
                Squares[r, c].IsValidMove = false;
                Squares[r, c].IsInCheck   = false;
                Squares[r, c].IsLastMove  = false;
            }
    }

    private void RefreshBoard()
    {
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                Squares[r, c].PieceSymbol = _board.GetPiece(r, c)?.Symbol ?? "";

        if (_lastMove != null)
        {
            Squares[_lastMove.FromRow, _lastMove.FromCol].IsLastMove = true;
            Squares[_lastMove.ToRow,   _lastMove.ToCol  ].IsLastMove = true;
        }
    }

    // ----------------------------------------------------------------
    // Desistir
    // ----------------------------------------------------------------
    private async Task OnResign()
    {
        if (_gameOver || ResignRequested == null) return;
        bool confirmed = await ResignRequested.Invoke();
        if (!confirmed) return;

        StopClock();
        StatusMessage = IsTournamentMode
            ? $"Você desistiu. {TournamentOpponent} vence!"
            : "Você desistiu. Pretas (IA) vencem!";
        if (IsTournamentMode) SetTournamentResult(false);
        GameOver = true;
        _sound.PlayGameOver();
    }

    // ----------------------------------------------------------------
    // Oferta de empate (apenas fora de torneio)
    // ----------------------------------------------------------------
    private async Task OnOfferDraw()
    {
        if (_gameOver || IsTournamentMode || DrawOfferRequested == null) return;
        if (_isAIThinking) return;

        // IA aceita empate com ~30% de chance (mais provável se estiver em desvantagem)
        bool aiAccepts = await DrawOfferRequested.Invoke();
        if (!aiAccepts) return;

        StopClock();
        StatusMessage = "Empate acordado!";
        GameOver = true;
        _sound.PlayGameOver();
    }

    private void UpdateStatus(GameState state)
    {
        switch (state)
        {
            case GameState.Checkmate:
                // CurrentTurn = quem está em xeque-mate (perdeu)
                bool whiteCheckmated = _board.CurrentTurn == PieceColor.White;
                var winnerName = whiteCheckmated
                    ? (IsTournamentMode ? TournamentOpponent : "Pretas (IA)")
                    : "Brancas";
                StatusMessage = $"Xeque-Mate! {winnerName} vencem!";
                if (IsTournamentMode) SetTournamentResult(!whiteCheckmated);
                GameOver = true;
                StopClock();
                break;

            case GameState.Stalemate:
                // Em torneio: afogamento conta como derrota do jogador
                StatusMessage = IsTournamentMode
                    ? $"Afogamento! {TournamentOpponent} vence por regra!"
                    : "Afogamento! Empate!";
                if (IsTournamentMode) SetTournamentResult(false);
                GameOver = true;
                StopClock();
                break;

            case GameState.Draw:
                // Em torneio: empate por 50 lances / repetição / material = derrota do jogador
                string drawReason = _board.HalfMoveClock >= 100 ? "Regra dos 50 lances"
                    : ChessEngine.IsInsufficientMaterial(_board) ? "Material insuficiente"
                    : "Repetição de posição";
                StatusMessage = IsTournamentMode
                    ? $"Empate por {drawReason}. {TournamentOpponent} avança!"
                    : $"Empate! ({drawReason})";
                if (IsTournamentMode) SetTournamentResult(false);
                GameOver = true;
                StopClock();
                break;

            case GameState.Check:
                var (kr, kc) = _board.FindKing(_board.CurrentTurn);
                if (kr >= 0) Squares[kr, kc].IsInCheck = true;
                StatusMessage = _board.CurrentTurn == PieceColor.White
                    ? "Xeque! Sua vez (Brancas)"
                    : "Xeque! IA pensando...";
                break;

            default:
                StatusMessage = _board.CurrentTurn == PieceColor.White
                    ? "Sua vez (Brancas)"
                    : "IA pensando...";
                break;
        }
    }

    private static string FormatTime(TimeSpan t) =>
        $"{(int)t.TotalMinutes:D2}:{t.Seconds:D2}";

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPC([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
