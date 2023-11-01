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
	[Export] private Color _slashColor;
	[Export] private AudioStreamPlayer2D _slashSound;
	private CancellationTokenSource _cts;
	private CompositeDisposable _disposable;
	private CooldownTimer _timer;
	private bool _isCharging;
	private bool _isSlashing;
	private int _chargeMaxLevel = 0;
	private int _baseDamage = 10;
	private float _baseChargeDuration = 1.0f;
	private float _baseCooldown = 0.5f;
	private Tween _chargeTween;
	private Tween _slashTween;
	public override void Initialize(WeaponHand weaponHand)
	{
		_cts = new CancellationTokenSource();
		_disposable = new CompositeDisposable();
		_timer = new CooldownTimer();
		
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
		const float baseRadius = 0.128f;
		const float radiusRatio = 0.4f;
		const float durationRatio = 0.2f;
		var radius = MathfExtensions.LinearGrowth(baseRadius, radiusRatio ,_chargeMaxLevel);
		var duration = MathfExtensions.LinearGrowth(_baseChargeDuration, durationRatio, _chargeMaxLevel);
		_chargeTween = CreateTween();
		var size = new Vector2(radius, radius);
		_chargeTween.TweenProperty(_slashArea, "scale", size, duration)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		try
		{
			await _chargeTween.PlayAsync(ct);
		}
		catch (TaskCanceledException)
		{
			GD.Print("Charge canceled");
		}
	}

	private async GDTask Slash(CancellationToken ct)
	{
		_isCharging = false;
		_isSlashing = true;
		_chargeTween?.Kill();
		_slashTween?.Kill();
		_timer.CountAsync(_baseCooldown, ct).Forget();
		_slashSound.Play();
		_slashSprite.Modulate = _slashColor;
		_slashTween = CreateTween();
		_slashTween.TweenProperty(_slashSprite, "modulate:a", 0.0f, 0.25f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out); 
		DamageToArea();
		await _slashTween.PlayAsync(ct);
		_slashArea.Scale = new Vector2(0.0f, 0.0f);
		_isSlashing = false;
	}

	private void DamageToArea()
	{
		// 範囲内の敵にダメージを与える
		var bodies = _slashArea.GetOverlappingBodies();
		var enemies = bodies.OfType<IEnemy>();
		foreach (var enemy in enemies)
		{
			enemy.TakeDamage(_baseDamage);
		}
	}
	
	public void IncreaseChargeLevel()
	{
		_chargeMaxLevel++;
	}
	
	public void ReduceChargeDuration()
	{
		_baseChargeDuration *= 0.85f;
		_baseCooldown *= 0.9f;
	}
	
	public void IncreaseDamage()
	{
		_baseDamage += 2;
	}
}
