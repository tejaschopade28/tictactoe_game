
public static class GameEnums
{
    public enum GameMode
    {
        VsOnlinePlayer,
        VsComputer,
        VsOfflinePlayer
    }
    public enum TurnState
    {
        playerTurn,
        computerTurn,
        onlinePlayerTurn
    }
    
    public enum PlayerType
    {
        empty,
        X,
        O
    };
    public enum GameState
    {
        ModeSelection,
        WaitingRoom,
        OfflineMode,
        Playing,
        GameOver,
        Draw

    }
    public enum RematchState
{
    None,
    IAccepted,
    OpponentAccepted,
    BothAccepted
}



}