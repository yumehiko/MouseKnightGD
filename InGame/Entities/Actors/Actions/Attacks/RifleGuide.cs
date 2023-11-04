using System.Collections.Generic;
using Godot;
using photon.Core;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

public partial class RifleGuide : Node2D
{
    private IReadOnlyList<Vector2> _barrels;
    private bool _inCooldown = false;

    public void Initialize(IReadOnlyList<Vector2> barrels)
    {
        _barrels = barrels;
    }
    
    public override void _Draw()
    {
        foreach (var angle in _barrels)
        {
            DrawBarrelGuide(angle);
        }
    }

    private void DrawBarrelGuide(Vector2 angleVector)
    {
        const float offset = 48.0f; // 原点からのオフセット
        const float length = 32.0f; // 線の長さ
        
        var color = _inCooldown ? ColorPalette.GuideInactive : ColorPalette.GuideActive;
        var angle = angleVector.Angle();
        var start = new Vector2(offset, 0).Rotated(angle);
        var end = new Vector2(offset + length, 0).Rotated(angle);
        DrawLine(start, end, color, 0.5f, true);
    }
    
    public void AddGuide(Vector2 barrelAngle)
    {
        QueueRedraw();
    }
    
    public void SetCooldownColor(bool inCooldown)
    {
        _inCooldown = inCooldown;
        QueueRedraw();
    }
}