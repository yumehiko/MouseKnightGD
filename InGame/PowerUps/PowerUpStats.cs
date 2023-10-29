using Godot;
using System.Collections.Generic;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.PowerUps;

public abstract partial class PowerUpStats : PowerUpBase
{
    [Export] protected PowerUpStats[] NextPowerUps;

    public abstract void Initialize(AttackBase weaponInstance);
}