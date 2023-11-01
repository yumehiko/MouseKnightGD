namespace photon.InGame;

public class GameSessionResult
{
    public uint Score { get; }
    
    public GameSessionResult(uint score)
    {
        Score = score;
    }
}