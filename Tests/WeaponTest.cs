using System;
using System.Reactive;
using System.Reactive.Subjects;
using Godot;
using photon.InGame.Entities.Actors.Actions.Attacks;
using photon.InGame.Entities.Actors.Brains;
using photon.InGame.Entities.Actors.Heroes;
using Reactive.Bindings;

namespace photon.Tests;

public partial class WeaponTest : Node2D, IWeaponHand
{
	[Export] private PlayerBrain _brain;
	[Export] private AttackBase _weapon;
	[Export] private Node2D _projectileRoot;

	public int WeaponCount { get; }
	public Node2D ProjectileRoot => _projectileRoot;
	public IReadOnlyReactiveProperty<bool> LeftTrigger => _brain.LeftTrigger;
	public bool IsDead { get; }
	public IObservable<Unit> OnDeath { get; } = new Subject<Unit>();
	public void AddWeapon(AttackBase weapon)
	{
		throw new NotImplementedException();
	}

	public override void _Ready()
	{
		base._Ready();
		_weapon.Initialize(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Position = _brain.WayPoint.Value;
	}
}
