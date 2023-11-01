namespace photon.Title;

public class TitleSessionResult
{
    public ResultType Type { get; }
    public TitleSessionResult(ResultType resultType)
    {
        Type = resultType;
    }
    
    
    public enum ResultType
    {
        BeginGame,
        Exit,
    }
}
