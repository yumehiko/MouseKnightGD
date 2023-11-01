using System.Collections.Generic;
using photon.InGame.Entities.Actors.Actions.Attacks;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.PowerUps;

public partial class PuSlashCd : PowerUpStats
{
    private Sword _sword;

    public override void Initialize(AttackBase weaponInstance)
    {
        _sword = (Sword) weaponInstance;
    }

    public override void Apply(WeaponHand weaponHand)
    {
        _sword.IncreaseCooldownReductionRateOnCritical();
    }

    public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
    {
        return null;
    }
}