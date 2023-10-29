using System.Collections.Generic;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PuRifleRapid : PowerUpStats
{
    
    private Rifle _rifle;
    public override void Initialize(AttackBase weaponInstance)
    {
        _rifle = (Rifle) weaponInstance;
    }
    public override void Apply(WeaponHand weaponHand)
    {
        _rifle.AccelerationFireRate();
    }

    public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
    {
        return null;
    }
}