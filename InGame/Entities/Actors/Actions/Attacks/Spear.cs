using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using MouseKnightGD.InGame.Entities.Enemies;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

/// <summary>
///     スピア
///     マウスクリックを離したとき、最も近い敵を刺突する範囲攻撃。
/// </summary>
public partial class Spear : AttackBase
{
	[Export] private Area2D _findArea;
	[Export] private Node2D _bladePivot;
	[Export] private Node2D _visualPivot;
	[Export] private PackedScene _bladePrefab;
	[Export] private PackedScene _bladeVisualPrefab;
	
	private readonly float _baseCooldownTime = 0.8f;
	private int _bloodPowerCount;
	private float _cooldownTime = 0.8f;
	private CancellationTokenSource _cts;
	private int _damage = 2;
	private CompositeDisposable _disposable;
	private bool _isAttackTriggered;
	private int _reduceCooldownPowerCount;
	private CooldownTimer _timer;
	private readonly List<SpearBlade> _blades = new List<SpearBlade>();

	public override void Initialize(WeaponHand weaponHand)
	{
		_timer = new CooldownTimer();
		_cts = new CancellationTokenSource();
		_disposable = new CompositeDisposable();
		AddBlade(true);
		weaponHand.LeftTrigger
			.Where(_ => !weaponHand.IsDead)
			.Where(isOn => !isOn)
			.Skip(1)
			.Subscribe(_ => Attack()).AddTo(_disposable);
	}

	public override void _ExitTree()
	{
		_disposable?.Dispose();
		base._ExitTree();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		TowardNearestEnemyPoint();
		GiveDamageToArea();
	}

	private void Attack()
	{
		if (_timer.InCooldown.Value) return;
		_isAttackTriggered = true;
	}

	private void GiveDamageToArea()
	{
		if (!_isAttackTriggered) return;
		var blooded = false;
		foreach (var blade in _blades)
		{
			var killed = blade.Attack(_damage);
			if (killed) blooded = true;
		}

		_isAttackTriggered = false;
		var cooldown = blooded ? BloodedCooldown() : _cooldownTime;
		_timer.CountAsync(cooldown, _cts.Token).Forget();
	}

	private void TowardNearestEnemyPoint()
	{
		var targets = _findArea.GetOverlappingBodies().OfType<EnemyBase>();
		targets = targets.OrderBy(x => GlobalPosition.DistanceTo(x.GlobalPosition));
		var nearest = targets.FirstOrDefault();
		if (nearest == null) return;
		_bladePivot.LookAt(nearest.GlobalPosition);
	}
	public void ReduceBaseCooldown()
	{
		const float reduceRatio = 0.15f;
		_reduceCooldownPowerCount++;
		_cooldownTime = _baseCooldownTime * (1.0f / (1.0f + _reduceCooldownPowerCount * reduceRatio));
	}

	public void AddBlade(bool isInitial = false)
	{
		var blade = _bladePrefab.Instantiate<SpearBlade>();
		_bladePivot.AddChild(blade);
		var visual = _bladeVisualPrefab.Instantiate<SpearBladeVisual>();
		_visualPivot.AddChild(visual);
		blade.Initialize(visual, isInitial);
		_blades.Add(blade);
	}

	public void IncreaseBloodPower()
	{
		_bloodPowerCount++;
	}

	private float BloodedCooldown()
	{
		const float bloodRatio = 0.4f;
		var bloodedCooldown = _cooldownTime * (1.0f / (1.0f + _bloodPowerCount * bloodRatio));
		return bloodedCooldown;
	}
}
