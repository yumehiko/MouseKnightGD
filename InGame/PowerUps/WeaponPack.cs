using System.Collections.Generic;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.PowerUps;

public partial class WeaponPack : PowerUpBase
{
    /// <summary>
    /// この武器の実体。
    /// </summary>
    [Export] private PackedScene _attackPrefab;
    public override void Apply(Hero hero)
    {
        var instance = _attackPrefab.Instantiate<AttackBase>();
        hero.AddWeapon(instance);
    }

    public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
    {
        return NextPowerUps;
    }
}