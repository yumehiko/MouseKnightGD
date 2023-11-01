using Godot;
using photon.InGame.Entities.Actors.Actions.Attacks;

namespace photon.InGame.PowerUps;

public abstract partial class PowerUpStats : PowerUpBase
{
    [Export] protected PowerUpStats[] NextPowerUps;

    public abstract void Initialize(AttackBase weaponInstance);
}