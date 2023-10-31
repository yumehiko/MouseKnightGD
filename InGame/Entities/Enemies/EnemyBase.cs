using System;
using System.Reactive;
using System.Reactive.Subjects;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.Entities.Enemies;

public abstract partial class EnemyBase : RigidBody2D, IEnemy
{
	[Export] private EnemyVisual _visual;
	[Export] private int _maxHp = 1;
	private int _hp;
	private readonly Subject<Unit> _onDead = new Subject<Unit>();
	private readonly Subject<Unit> _onRemove = new Subject<Unit>();
	protected Hero Player;
	public bool IsDead => _hp <= 0;
	public IObservable<Unit> OnDeath => _onDead;
	public IObservable<Unit> OnRemove => _onRemove;
	public int Fun { get; set; }

	public virtual void Initialize(Vector2 spawnPosition, Hero player)
	{
		_hp = _maxHp;
		Player = player;
		Position = spawnPosition;
		Visible = true;
	}

	public void Remove()
	{
		_onRemove.OnCompleted();
		QueueFree();
	}

	public bool TakeDamage(int amount)
	{
		if (IsDead) return false;
		_hp -= amount;
		var normalized = (float) _hp / _maxHp;
		_visual.Damage(normalized);
		if (!IsDead) return false;
		Die();
		return true;
	}
	
	public void Die()
	{
		_onDead.OnCompleted();
		Remove();
	}

	protected void GiveDamage(Node body)
	{
		if (body is Hero hero)
		{
			hero.TakeDamage(1);
		}
	}
}
