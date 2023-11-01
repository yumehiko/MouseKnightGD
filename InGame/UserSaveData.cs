using Godot;

namespace photon.InGame;

public class UserSaveData
{
    public uint HighScore { get; } = 0;
    
    public UserSaveData()
    {
        var saveData = FileAccess.Open("user://highScore.save", FileAccess.ModeFlags.Read);
        if (saveData == null) return;
        HighScore = saveData.Get32();
        saveData.Close();
        saveData.Dispose();
    }
    
    public void WriteHighScore(uint score)
    {
        if (score <= HighScore) return;
        var saveData = FileAccess.Open("user://highScore.save", FileAccess.ModeFlags.Write);
        saveData.Store32(score);
        saveData.Close();
        saveData.Dispose();
    }
}