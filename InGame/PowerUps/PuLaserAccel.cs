using System.Collections.Generic;
using photon.InGame.Entities.Actors.Actions.Attacks;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.PowerUps;

public partial class PuLaserAccel : PowerUpStats
{
    private Laser _laser;
    public override void Initialize(AttackBase weaponInstance)
    {
        _laser = (Laser) weaponInstance;
    }

    public override void Apply(WeaponHand weaponHand)
    {
        _laser.Accel();
    }

    public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
    {
        return null;
    }
}