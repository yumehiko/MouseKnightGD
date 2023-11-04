using Godot;
using photon.Core;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

public partial class LaserGuide : Node2D
{
    private bool _inCooldown = false;
    private int _chargeGuideAmount = 0;

    public override void _Draw()
    {
        DrawGuideLine();
        DrawChargeGuideLine();
    }

    private void DrawGuideLine()
    {
        const float length = 4800.0f;
        const float offset = 40.0f;
        var from = new Vector2(offset, 0);
        var to = new Vector2(length - offset, 0);
        var color = _inCooldown ? ColorPalette.GuideInactive : ColorPalette.GuideActive;
        DrawLine(from, to, color, 0.5f, true);
    }

    private void DrawChargeGuideLine()
    {
        if (_chargeGuideAmount == 0) return;
        const float ratio = 12.0f;
        const float offset = 40.0f;
        var length = _chargeGuideAmount * ratio + offset;
        var from = new Vector2(offset, 0);
        var to = new Vector2(length, 0);
        var color = ColorPalette.ChargeColor;
        DrawLine(from, to, color, 0.5f, true);
    }

    public void SetCooldownColor(bool inCooldown)
    {
        _inCooldown = inCooldown;
        QueueRedraw();
    }

    public void SetChargeGuideAmount(int amount)
    {
        _chargeGuideAmount = amount;
        QueueRedraw();
    }
}