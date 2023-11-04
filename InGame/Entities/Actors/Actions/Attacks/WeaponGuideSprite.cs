using Godot;
using photon.Core;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

public partial class WeaponGuideSprite : Sprite2D
{
    private bool _inCooldown = false;

    public void SetCooldownColor(bool inCooldown)
    {
        Modulate = inCooldown ? ColorPalette.GuideInactive : ColorPalette.GuideActive;
    }
}