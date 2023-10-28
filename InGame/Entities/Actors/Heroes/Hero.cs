using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Brains;
using MouseKnightGD.InGame.Entities.Chips;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame.Entities.Actors.Heroes;

/// <summary>
/// ヒーロー。このゲームにおけるプレイヤーキャラクター。
/// </summary>
public partial class Hero : RigidBody2D, IEntity, IDamageable, IDieable
{
	[Export] private HeroVisual _visual;
	[Export] private ChipCollector _chipCollector;
	private Node2D _projectileRoot;
	private List<AttackBase> _weapons;

	private CompositeDisposable _disposable;
	private Subject<Unit> _onRemove;
	public int WeaponCount => _weapons.Count;
	public IReadOnlyReactiveProperty<int> Chips => _chipCollector.Chips;
	public Node2D ProjectileRoot => _projectileRoot;
	public Health Health { get; private set; }
	public IBrain Brain { get; private set; }
	public bool IsDead => Health.IsDead;
	public IObservable<Unit> OnDeath => Health.OnDeath;
	public IObservable<Unit> OnRemove => _onRemove;

	private bool _isActive;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Initialize(IBrain brain, Node2D projectileRoot)
	{
		_projectileRoot = projectileRoot;
		Brain = brain;
		Health = new Health(3);
		_weapons = new List<AttackBase>();
		_disposable = new CompositeDisposable();
		_disposable.Add(Health);
		_chipCollector.Initialize();
		_visual.Initialize(Health, _chipCollector);
		_onRemove = new Subject<Unit>().AddTo(_disposable);
		Health.OnDeath.Subscribe(_ => { }, Remove).AddTo(_disposable);
		_isActive = true;
	}
	
	public void Remove()
	{
		// このヒーローを削除する……ただしQueueFree()は呼ばない
		_onRemove.OnCompleted();
		_chipCollector.Disable();
		_disposable.Dispose();
		_isActive = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (!_isActive) return;
		Position = Brain.WayPoint.Value;
	}
	
	public void SubChips(int amount) => _chipCollector.SubChips(amount);

	public void TakeDamage(int amount) => Health.TakeDamage(1);
	public void Die() => Health.Die();

	public void AddWeapon(AttackBase weapon)
	{
		AddChild(weapon);
		weapon.Initialize(this);
		_weapons.Add(weapon);
	}
}
