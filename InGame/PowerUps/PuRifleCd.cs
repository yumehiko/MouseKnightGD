using System.Collections.Generic;
using photon.InGame.Entities.Actors.Actions.Attacks;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.PowerUps;

public partial class PuRifleCd : PowerUpStats
{
	private Rifle _rifle;
	public override void Initialize(AttackBase weaponInstance)
	{
		_rifle = (Rifle) weaponInstance;
	}
	public override void Apply(WeaponHand weaponHand)
	{
		_rifle.IncreaseFireRate();
	}

	public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
	{
		return null;
	}
}
