using System;
using System.Reactive;
using System.Reactive.Subjects;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Brains;
using MouseKnightGD.InGame.Entities.Chips;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame.Entities.Actors.Heroes;

/// <summary>
/// ヒーロー。このゲームにおけるプレイヤーキャラクター。
/// </summary>
public partial class Hero : RigidBody2D, IEntity, IDamageable, IDieable
{
	[Export] private HeroVisual _visual;

	private CompositeDisposable _disposable;
	private Subject<Unit> _onRemove;
	public Health Health { get; private set; }
	public IBrain Brain { get; private set; }
	public bool IsDead => Health.IsDead;
	public IObservable<Unit> OnDeath => Health.OnDeath;
	public IObservable<Unit> OnRemove => _onRemove;

	private bool _isActive;

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Initialize(IBrain brain)
	{
		_disposable = new CompositeDisposable();
		_onRemove = new Subject<Unit>();
		Brain = brain;
		Health = new Health(3);
		_visual.Initialize(Health);
		Health.OnDeath.Subscribe(_ => { }, Remove).AddTo(_disposable);
		_isActive = true;
	}
	
	public void Remove()
	{
		// このヒーローを削除する……ただしQueueFree()は呼ばない
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

	private void OnAreaEntered(Node2D body)
	{
		if (body is Chip chip)
		{
			// TODO: ここでチップを回収する処理を書く
			chip.QueueFree();
		}
	}

	public void TakeDamage(int amount) => Health.TakeDamage(1);
	public void Die() => Health.Die();
}
