using System;
using System.Reactive;
using Godot;
using photon.InGame.Entities.Actors.Actions.Attacks;
using Reactive.Bindings;

namespace photon.InGame.Entities.Actors.Heroes;

public interface IWeaponHand
{
    int WeaponCount { get; }
    Node2D ProjectileRoot { get; }
    IReadOnlyReactiveProperty<bool> LeftTrigger { get; }
    bool IsDead { get; }
    IObservable<Unit> OnDeath { get; }
    void AddWeapon(AttackBase weapon);
}