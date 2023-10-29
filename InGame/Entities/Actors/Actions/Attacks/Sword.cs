using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using MouseKnightGD.InGame.Entities.Enemies;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

/// <summary>
/// スラッシュ。
/// マウスクリック時、使用者の上方向へ範囲攻撃を行う。
/// </summary>
public partial class Sword : AttackBase
{
	[Export] private Area2D _area;
	[Export] private Sprite2D _visual;
	[Export] private CpuParticles2D _particles;
	private CooldownTimer _timer;
	private CancellationTokenSource _cts;
	private CompositeDisposable _disposable;
	private bool _isAttackTriggered;
	private Vector2 _slashPoint;
	private int _damage = 4;
	private float _criticalRate = 0.0f;
	private float _cooldownReductionRateOnCritical = 0.0f;
	private float _baseCooldown = 0.5f;
	private float ReducedCooldown => _baseCooldown * (1.0f / (1.0f + _cooldownReductionRateOnCritical));
	private bool _isCritical;

	public override void Initialize(WeaponHand weaponHand)
	{
		_timer = new CooldownTimer();
		_cts = new CancellationTokenSource();
		Position = new Vector2(0, -32);
		_disposable = new CompositeDisposable();
		weaponHand.LeftTrigger
			.Where(_ => !weaponHand.IsDead)
			.Where(isOn => isOn)
			.Subscribe(x => Attack(_cts.Token)).AddTo(_disposable);
	}

	public override void _ExitTree()
	{
		_disposable?.Dispose();
		base._ExitTree();
	}

	public override void _PhysicsProcess(double delta)
	{
		GiveDamageToArea();
		base._PhysicsProcess(delta);
	}

	private void Attack(CancellationToken ct)
	{
		if(!_timer.InCooldown.Value) _isAttackTriggered = true;
		_isCritical = GD.Randf() <= _criticalRate;
		var cooldown = _isCritical ? ReducedCooldown : _baseCooldown;
		_timer.CountAsync(cooldown, ct).Forget();
	}

	private void GiveDamageToArea()
	{
		if (!_isAttackTriggered) return;
		SlashAnimation();
		// 範囲内の敵にダメージを与える
		var bodies = _area.GetOverlappingBodies();
		var enemies = bodies.OfType<IEnemy>();
		var damage = _isCritical ? _damage * 2 : _damage;
		foreach (var enemy in enemies)
		{
			enemy.TakeDamage(damage);
		}

		if (_isCritical)
		{
			_particles.Restart();
		}
		_isCritical = false;
		_isAttackTriggered = false;
	}
	
	private void SlashAnimation()
	{
		// _visualを表示する
		_visual.Show();
		_visual.Modulate = Colors.White;
		// _visualをTweenで徐々にフェードアウトする
		var tween = CreateTween();
		tween.TweenProperty(_visual, "modulate:a", 0, 0.2f);
		tween.Play();
	}
	
	// --- パワーアップ ---
	
	public void IncreaseDamage()
	{
		_damage += 1;
	}
	
	public void IncreaseCriticalRate()
	{
		_criticalRate += 0.1f;
	}
	
	public void IncreaseCooldownReductionRateOnCritical()
	{
		_cooldownReductionRateOnCritical += 0.2f;
	}
}
