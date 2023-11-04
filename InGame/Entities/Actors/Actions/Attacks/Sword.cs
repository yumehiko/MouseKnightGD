using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Fractural.Tasks;
using Godot;
using photon.InGame.Entities.Actors.Heroes;
using photon.InGame.Entities.Enemies;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

/// <summary>
/// スラッシュ。
/// マウスクリック時、使用者の上方向へ範囲攻撃を行う。
/// </summary>
public partial class Sword : AttackBase
{
	[Export] private Area2D _area;
	[Export] private Sprite2D _visual;
	[Export] private WeaponGuideSprite _guide;
	[Export] private CpuParticles2D _particles;
	[Export] private AudioStreamPlayer2D _se;
	private CooldownTimer _timer;
	private CancellationTokenSource _cts;
	private CompositeDisposable _disposable;
	private bool _isAttackTriggered;
	private Vector2 _slashPoint;
	private int _damage = 18;
	private float _criticalRate;
	private float _cooldownReductionRateOnCritical;
	private float _baseCooldown = 1.0f;
	private float ReducedCooldown => _baseCooldown * (1.0f / (1.0f + _cooldownReductionRateOnCritical));
	private bool _isCritical;

	public override void Initialize(IWeaponHand weaponHand)
	{
		const float offset = -48.0f;
		Position = new Vector2(0, offset);
		_timer = new CooldownTimer();
		_cts = new CancellationTokenSource();
		_disposable = new CompositeDisposable();
		weaponHand.LeftTrigger
			.Where(_ => !weaponHand.IsDead)
			.Where(isOn => isOn)
			.Subscribe(_ => Attack(_cts.Token)).AddTo(_disposable);
		
		_timer.InCooldown
			.Where(_ => !weaponHand.IsDead)
			.Subscribe(inCd => _guide.SetCooldownColor(inCd)).AddTo(_disposable);

		weaponHand.OnDeath.Subscribe(_ => { }, Disable);
	}

	private void Disable()
	{
		Hide();
		_cts?.Cancel();
		_cts?.Dispose();
		_disposable?.Dispose();
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
		_se.Play();
		// _visualをTweenで徐々にフェードアウトする
		var tween = CreateTween();
		tween.TweenProperty(_visual, "modulate:a", 0, 0.2f);
		tween.Play();
	}
	
	// --- パワーアップ ---
	
	public void IncreaseDamage()
	{
		_damage += 5;
	}
	
	public void IncreaseCriticalRate()
	{
		_criticalRate += 0.2f;
	}
	
	public void IncreaseCooldownReductionRateOnCritical()
	{
		_cooldownReductionRateOnCritical += 0.25f;
	}
}
