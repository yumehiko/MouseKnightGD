using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;
using Godot.Extensions;
using photon.InGame.Entities.Actors.Heroes;
using photon.InGame.Entities.Enemies;
using Reactive.Bindings.Extensions;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

public partial class Axe : AttackBase
{
	[Export] private Area2D _slashArea;
	[Export] private Sprite2D _slashSprite;
	[Export] private Sprite2D _chargeSprite;
	[Export] private AxeGuide _guide;
	[Export] private Color _slashColor;
	[Export] private Color _chargingColor;
	[Export] private Color _maxChargedColor;
	[Export] private AudioStreamPlayer2D _slashSound;
	[Export] private AudioStreamPlayer2D _chargeSound;
	[Export] private CpuParticles2D _criticalEffect;
	private CancellationTokenSource _cts;
	private CompositeDisposable _disposable;
	private CooldownTimer _timer;
	private bool _isCharging;
	private bool _isMaxCharged;
	private bool _isSlashing;
	private int _baseDamage = 48;
	private int _chargeMaxLevel = 0;
	private float _maxRadius = 0.128f;
	private float _baseChargeDuration = 1.0f;
	private const float DurationRatio = 0.35f;
	private float _chargeDuration = 1.0f;
	private float _baseCooldown = 1.0f;
	private Tween _chargeTween;
	private Tween _slashTween;

	public override void Initialize(IWeaponHand weaponHand)
	{
		_cts = new CancellationTokenSource();
		_disposable = new CompositeDisposable();
		_timer = new CooldownTimer();
		_guide.SetScale(_maxRadius);
		
		_timer.InCooldown
			.Where(_ => !weaponHand.IsDead)
			.Subscribe(inCd => _guide.SetCooldownColor(inCd)).AddTo(_disposable);
		
		// トリガーを押したとき、チャージを開始する。
		weaponHand.LeftTrigger
			.Where(_ => !weaponHand.IsDead)
			.Where(isOn => isOn)
			.Where(_ => !_timer.InCooldown.Value)
			.Where(_ => !_isSlashing)
			.Subscribe(_ => StartCharge(_cts.Token).Forget()).AddTo(_disposable);
		
		// または、クールダウンが終わったときトリガーしていれば、チャージを開始する。
		_timer.InCooldown
			.Where(_ => !weaponHand.IsDead)
			.Where(inCd => !inCd)
			.Where(_ => weaponHand.LeftTrigger.Value)
			.Where(_ => !_isSlashing)
			.Subscribe(_ => StartCharge(_cts.Token).Forget()).AddTo(_disposable);
		
		// トリガーを離したとき、スラッシュを放つ。
		weaponHand.LeftTrigger
			.Where(_ => !weaponHand.IsDead)
			.Where(isOn => !isOn)
			.Where(_ => _isCharging)
			.Subscribe(_ => Slash(_cts.Token).Forget()).AddTo(_disposable);

		weaponHand.OnDeath.Subscribe(_ => { }, Disable);
	}

	private void Disable()
	{
		Hide();
		_chargeTween?.Kill();
		_slashTween?.Kill();
		_cts?.Cancel();
		_cts?.Dispose();
		_disposable?.Dispose();
	}

	private async GDTask StartCharge(CancellationToken ct)
	{
		_isCharging = true;
		_chargeTween?.Kill();
		_chargeSprite.Modulate = _chargingColor;
		_chargeSound.Play();
		_chargeTween = CreateTween();
		var size = new Vector2(_maxRadius, _maxRadius);
		_chargeTween.TweenProperty(_slashArea, "scale", size, _chargeDuration)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);
		_chargeTween.TweenCallback(Callable.From(OnMaxCharge));
		_chargeTween.TweenProperty(_chargeSprite, "modulate", _chargingColor, 0.125f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		
		await _chargeTween.PlayAsync(ct);
	}

	private void OnMaxCharge()
	{
		_isMaxCharged = true;
		_chargeSprite.Modulate = _maxChargedColor;
	}

	private async GDTask Slash(CancellationToken ct)
	{
		_isCharging = false;
		_isSlashing = true;
		_chargeTween?.Kill();
		_slashTween?.Kill();
		_timer.CountAsync(_baseCooldown, ct).Forget();
		DamageToArea(_isMaxCharged);
		await SlashAnimation(_isMaxCharged, ct);
		_slashArea.Scale = new Vector2(0.0f, 0.0f);
		_isSlashing = false;
		_isMaxCharged = false;
	}

	private async GDTask SlashAnimation(bool isCritical, CancellationToken ct)
	{
		_chargeSound.Stop();
		_slashSound.Play();
		_slashSprite.Modulate = _slashColor;
		if (isCritical) _criticalEffect.Emitting = true;
		_slashTween = CreateTween();
		_slashTween.TweenProperty(_slashSprite, "modulate:a", 0.0f, 0.25f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out); 
		await _slashTween.PlayAsync(ct);
	}

	private void DamageToArea(bool isCritical)
	{
		// 範囲内の敵にダメージを与える
		var bodies = _slashArea.GetOverlappingBodies();
		var enemies = bodies.OfType<IEnemy>();
		var damage = isCritical ? _baseDamage * 2 : _baseDamage;
		foreach (var enemy in enemies)
		{
			enemy.TakeDamage(damage);
		}
	}
	
	public void IncreaseChargeLevel()
	{
		const float baseRadius = 0.128f;
		const float radiusRatio = 0.5f;
		_chargeMaxLevel++;
		_maxRadius = MathfExtensions.LinearGrowth(baseRadius, radiusRatio ,_chargeMaxLevel);
		_chargeDuration = MathfExtensions.LinearGrowth(_baseChargeDuration, DurationRatio, _chargeMaxLevel);
		
		_guide.SetScale(_maxRadius);
		
		var effectRadius = _maxRadius * 10.0f;
		var effectAmount = 32 + (16 * _chargeMaxLevel);
		_criticalEffect.Scale = new Vector2(effectRadius, effectRadius);
		_criticalEffect.Amount = effectAmount;
	}
	
	public void ReduceChargeDuration()
	{
		_baseChargeDuration *= 0.8f;
		_baseCooldown *= 0.9f;
		_chargeDuration = MathfExtensions.LinearGrowth(_baseChargeDuration, DurationRatio, _chargeMaxLevel);
	}
	
	public void IncreaseDamage()
	{
		_baseDamage += 16;
	}
}
