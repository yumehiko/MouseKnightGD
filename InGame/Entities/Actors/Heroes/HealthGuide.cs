using Godot;
using photon.Core;

namespace photon.InGame.Entities.Actors.Heroes;

public partial class HealthGuide : Node2D
{
    private int _maxHealth;
    private int _healthAmount;

    public void Initialize(int maxHealth)
    {
        _maxHealth = maxHealth - 1;
    }
    
    public override void _Draw()
    {
        // ヘルスの数だけ、円弧を描画する。
        // 完全な円を_maxHealth個のパーツに分割する。
        // 各パーツは1つの円弧でできている
        // 円弧の間に少し間隔をあける
        
        var radius = 42.0f;
        // gapは6度
        var gap = Mathf.Pi / 30;
        
        var angle = Mathf.Pi * 2 / _maxHealth - gap;
        var startAngle = 0.0f;
        var endAngle = startAngle + angle;
        var color = Colors.White;
        for (var i = 0; i < _maxHealth; i++)
        {
            if (i < _healthAmount - 1)
            {
                color = ColorPalette.HealthGuide;
            }
            else
            {
                color = ColorPalette.HealthGuideEmpty;
            }
            DrawArc(Vector2.Zero, radius, startAngle, endAngle, 8, color, 0.5f, true);
            startAngle += angle + gap;
            endAngle += angle + gap;
        }
    }

    public void SetHealthAmount(int amount)
    {
        _healthAmount = amount;
        QueueRedraw();
    }
}