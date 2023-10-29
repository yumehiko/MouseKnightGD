using System.Collections.Generic;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PuSlashCritical : PowerUpStats
{
    private Sword _sword;

    public override void Initialize(AttackBase weaponInstance)
    {
        _sword = (Sword) weaponInstance;
    }
    public override void Apply(WeaponHand weaponHand)
    {
        _sword.IncreaseCriticalRate();
    }

    public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
    {
        return null;
    }
}