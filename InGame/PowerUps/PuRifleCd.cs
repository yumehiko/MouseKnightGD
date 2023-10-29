using System.Collections.Generic;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PuRifleCd : PowerUpStats
{
    private Rifle _rifle;
    public override void Initialize(AttackBase weaponInstance)
    {
        _rifle = (Rifle) weaponInstance;
    }
    public override void Apply(WeaponHand weaponHand)
    {
        GD.Print("PowerUp: RifleCd");
    }

    public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
    {
        return null;
    }
}