using Godot;
using photon.Core;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

/// <summary>
/// Axeのガイド。Axeと同じ半径の線分の円を描画する。
/// </summary>
public partial class AxeGuide : Node2D
{
    private float _guideScale = 0.0f;
    private bool _inCooldown = false;

    public override void _Draw()
    {
        const float radiusBase = 500;
        var radius = radiusBase * _guideScale;
        var color = _inCooldown ? ColorPalette.GuideInactive : ColorPalette.GuideActive;
        DrawArc(Vector2.Zero, radius, 0, 360, 64, color, 0.5f, true);
    }
    
    public void SetScale(float scale)
    {
        _guideScale = scale;
        QueueRedraw();
    }

    public void SetCooldownColor(bool inCooldown)
    {
        _inCooldown = inCooldown;
        QueueRedraw();
    }
}