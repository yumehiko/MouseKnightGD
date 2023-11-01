using System;
using System.Collections.Generic;
using System.Reactive;
using Godot;
using photon.InGame.Entities.Actors.Actions.Attacks;
using Reactive.Bindings;

namespace photon.InGame.Entities.Actors.Heroes;

public class WeaponHand
{
    private readonly Hero _body;
    private readonly List<AttackBase> _weapons;
    public int WeaponCount => _weapons.Count;
    public Node2D ProjectileRoot { get; }
    
    public IReadOnlyReactiveProperty<bool> LeftTrigger => _body.Brain.LeftTrigger;
    public bool IsDead => _body.IsDead;
    public IObservable<Unit> OnDeath => _body.OnDeath; 
    
    public WeaponHand(Hero body, Node2D projectileRoot)
    {
        _body = body;
        ProjectileRoot = projectileRoot;
        _weapons = new List<AttackBase>();
    }

    public void AddWeapon(AttackBase weapon)
    {
        _body.AddChild(weapon);
        weapon.Initialize(this);
        _weapons.Add(weapon);
    }
}