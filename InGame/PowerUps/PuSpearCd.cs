using System.Collections.Generic;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PuSpearCd : PowerUpStats
{
	private Spear _spear;
	public override void Initialize(AttackBase weaponInstance)
	{
		_spear = (Spear) weaponInstance;
	}

	public override void Apply(WeaponHand weaponHand)
	{
		GD.Print("PowerUp: SpearCd");
	}

	public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
	{
		return null;
	}
}
