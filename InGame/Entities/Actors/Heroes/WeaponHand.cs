using System.Collections.Generic;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using Reactive.Bindings;

namespace MouseKnightGD.InGame.Entities.Actors.Heroes;

public class WeaponHand
{
    private readonly Hero _body;
    private readonly List<AttackBase> _weapons;
    public int WeaponCount => _weapons.Count;
    public Node2D ProjectileRoot { get; }
    
    public IReadOnlyReactiveProperty<bool> LeftTrigger { get; }
    public bool IsDead => _body.IsDead;
    
    public WeaponHand(Hero body, Node2D projectileRoot, IReadOnlyReactiveProperty<bool> leftTrigger)
    {
        _body = body;
        ProjectileRoot = projectileRoot;
        LeftTrigger = leftTrigger;
        _weapons = new List<AttackBase>();
    }

    public void AddWeapon(AttackBase weapon)
    {
        _body.AddChild(weapon);
        weapon.Initialize(this);
        _weapons.Add(weapon);
    }
}