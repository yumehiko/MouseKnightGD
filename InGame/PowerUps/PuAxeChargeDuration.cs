using System.Collections.Generic;
using photon.InGame.Entities.Actors.Actions.Attacks;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.PowerUps;

public partial class PuAxeChargeDuration : PowerUpStats
{
    private Axe _axe;
    public override void Initialize(AttackBase weaponInstance)
    {
        _axe = (Axe) weaponInstance;
    }

    public override void Apply(WeaponHand weaponHand)
    {
        _axe.ReduceChargeDuration();
    }

    public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
    {
        return null;
    }
}