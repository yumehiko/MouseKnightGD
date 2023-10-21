using System;
using System.Reactive;
using System.Reactive.Subjects;
using Godot;
using MouseKnightGD.Entities.Actors.Brains;
using MouseKnightGD.Entities.Chips;

namespace MouseKnightGD.Entities.Actors.Heroes;

/// <summary>
/// ヒーロー。このゲームにおけるプレイヤーキャラクター。
/// </summary>
public partial class Hero : RigidBody2D, IEntity, IDamageable
{
	[Export] private HeroVisual _visual;

	private Subject<Unit> _onRemove;
	public Health Health { get; private set; }
	public IObservable<Unit> OnRemove => _onRemove;
	public IBrain Brain { get; private set; }

	/// <summary>
	/// 初期化処理
	/// </summary>
	public void Initialize(IBrain brain)
	{
		_onRemove = new Subject<Unit>();
		Brain = brain;
		Health = new Health(3);
		_visual.Initialize(Health);
	}
	
	public void Remove()
	{
		// このヒーローを削除する
		_onRemove.OnCompleted();
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
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
}
