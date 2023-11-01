using Godot;
using photon.InGame;

namespace photon.Title;

public partial class HighScoreLabel : Label
{
    public void ShowHighScore()
    {
        var data = new UserSaveData();
        var score = data.HighScore;
        if (score == 0) return;
        Text = score.ToString();
    }
}