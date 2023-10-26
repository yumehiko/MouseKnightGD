using System;
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
	[Export] private Area2D _chipCollector;
	[Export] private AttackFactory _attackFactory;
	private Node2D _projectileRoot;

	private ReactivePropertySlim<int> _chips;
	private CompositeDisposable _disposable;
	private Subject<Unit> _onRemove;
	public IReadOnlyReactiveProperty<int> Chips => _chips;
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
		_disposable = new CompositeDisposable();
		_disposable.Add(Health);
		_visual.Initialize(Health);
		_onRemove = new Subject<Unit>().AddTo(_disposable);
		Health.OnDeath.Subscribe(_ => { }, Remove).AddTo(_disposable);
		_chips = new ReactivePropertySlim<int>(0).AddTo(_disposable);
		_chipCollector.BodyEntered += OnAreaEntered;
		_attackFactory.Initialize(this);
		_isActive = true;
	}
	
	public void Remove()
	{
		// このヒーローを削除する……ただしQueueFree()は呼ばない
		_chipCollector.BodyEntered -= OnAreaEntered;
		_onRemove.OnCompleted();
		_disposable.Dispose();
		_isActive = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (!_isActive) return;
		Position = Brain.WayPoint.Value;
	}
	
	public void SubChips(int amount) => _chips.Value -= amount;

	private void OnAreaEntered(Node2D body)
	{
		if (body is not Chip chip) return;
		_chips.Value += chip.Value;
		chip.QueueFree();
	}

	public void TakeDamage(int amount) => Health.TakeDamage(1);
	public void Die() => Health.Die();
}
