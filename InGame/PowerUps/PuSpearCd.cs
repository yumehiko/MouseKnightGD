using System.Collections.Generic;
using photon.InGame.Entities.Actors.Actions.Attacks;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.PowerUps;

public partial class PuSpearCd : PowerUpStats
{
	private Spear _spear;
	public override void Initialize(AttackBase weaponInstance)
	{
		_spear = (Spear) weaponInstance;
	}

	public override void Apply(WeaponHand weaponHand)
	{
		_spear.ReduceBaseCooldown();
	}

	public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
	{
		return null;
	}
}
