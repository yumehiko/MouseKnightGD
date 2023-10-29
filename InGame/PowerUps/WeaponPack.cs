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
    [Export] protected PowerUpStats[] NextPowerUps;
    public override void Apply(WeaponHand weaponHand)
    {
        var instance = _attackPrefab.Instantiate<AttackBase>();
        weaponHand.AddWeapon(instance);
        
        //　この武器に関するパワーアップを初期化する。
        if (NextPowerUps == null) return;
        foreach (var next in NextPowerUps)
        {
            next.Initialize(instance);
        }
    }

    public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
    {
        return NextPowerUps;
    }
}