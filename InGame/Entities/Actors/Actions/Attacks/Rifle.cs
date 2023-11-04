using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;
using photon.InGame.Entities.Actors.Heroes;
using Reactive.Bindings.Extensions;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

public partial class Rifle : AttackBase
{
	[Export] private PackedScene _bulletPack;
	[Export] private AudioStreamPlayer2D _shotSound;
	[Export] private RifleGuide _guide;
	private float _coolTimeMax = 1.5f;
	private float _coolTimeMin = 0.5f;
	private float _rapidFireRate = 0.25f;
	private int _accelerateFirePowerCount = 0;
	private int _increaseFireRatePowerCount = 0;
	private float _coolTime;
	private List<Vector2> _barrelAngles;
	private Node2D _projectileRoot;
	private CooldownTimer _timer;
	private CompositeDisposable _disposable;
	private CancellationTokenSource _cts;

	public override void Initialize(IWeaponHand weaponHand)
	{
		_timer = new CooldownTimer();
		_disposable = new CompositeDisposable();
		_cts = new CancellationTokenSource();
		_projectileRoot = weaponHand.ProjectileRoot;
		_barrelAngles = new List<Vector2>();
		_guide.Initialize(_barrelAngles);
		// 上下左右にバレルアングルを設定する
		AddBarrel(Vector2.Up);
		AddBarrel(Vector2.Down);
		AddBarrel(Vector2.Left);
		AddBarrel(Vector2.Right);
		
		_coolTime = _coolTimeMax;
		
		_timer.InCooldown
			.Where(_ => !weaponHand.IsDead)
			.Subscribe(inCd => _guide.SetCooldownColor(inCd)).AddTo(_disposable);
		
		weaponHand.LeftTrigger
			.Where(_ => !weaponHand.IsDead)
			.Where(isOn => isOn)
			.Where(_ => !_timer.InCooldown.Value)
			.Subscribe(_ => Shot()).AddTo(_disposable);
		
		weaponHand.LeftTrigger
			.Where(_ => !weaponHand.IsDead)
			.Where(isOn => !isOn)
			.Subscribe(_ => ReleaseTrigger()).AddTo(_disposable);
		
		_timer.InCooldown
			.Where(_ => !weaponHand.IsDead)
			.Where(inCd => !inCd)
			.Where(_ => weaponHand.LeftTrigger.Value)
			.Subscribe(_ => Shot()).AddTo(_disposable);
		
		weaponHand.OnDeath.Subscribe(_ => { }, Disable);
	}

	private void Disable()
	{
		Hide();
		_disposable?.Dispose();
	}

	private void ReleaseTrigger()
	{
		_coolTime = _coolTimeMax;
	}

	private void Shot()
	{
		// すべての銃口で発射
		foreach (var barrelAngle in _barrelAngles)
		{
			var instance = _bulletPack.Instantiate<Bullet>();
			_projectileRoot.AddChild(instance);
			instance.Shot(GlobalPosition, barrelAngle, 640.0f, 6);
		}
		_shotSound.Play();
		_timer.CountAsync(_coolTime, _cts.Token).Forget();
		_coolTime = Mathf.Max(_coolTimeMin, _coolTime - _rapidFireRate);
	}
	
	// --- パワーアップ用 ---

	private void AddBarrel(Vector2 angle)
	{
		_barrelAngles.Add(angle);
	}
	
	public void AddBarrel()
	{
		for (var i = 0; i < 2; i++)
		{
			var randomX = GD.Randf() * 2.0f - 1.0f;
			var randomY = GD.Randf() * 2.0f - 1.0f;
		
			var randomAngle = new Vector2(randomX, randomY).Normalized();
			AddBarrel(randomAngle);
			_guide.AddGuide(randomAngle);
		}
	}
	
	
	/// <summary>
	/// 最大連射速度までに至る加速度を強化する。
	/// </summary>
	public void AccelerationFireRate()
	{
		const float baseRapidFireRate = 0.25f;
		const float accelerateRapidFireRate = 0.25f;
		// 強化するごとに最大クールタイムが減少し、RapidFireRateが増加する（＝最大連射速度に至るまでの加速度が増加する）
		_accelerateFirePowerCount++;
		_rapidFireRate = baseRapidFireRate * (1.0f + accelerateRapidFireRate * _accelerateFirePowerCount);
		ReCalculateMaxCoolTime();
	}
	
	private void ReCalculateMaxCoolTime()
	{
		const float baseMaxCooldown = 1.5f;
		const float reductionMaxCooldownRate = 1.2f;
		var logicalMaxCooldown = baseMaxCooldown / Mathf.Pow(reductionMaxCooldownRate, _accelerateFirePowerCount);
		_coolTimeMax = Mathf.Max(_coolTimeMin, logicalMaxCooldown);
	}
	
	public void IncreaseFireRate()
	{
		const float baseMinCooldown = 0.5f;
		const float reduceRatio = 0.12f;
		_increaseFireRatePowerCount++;
		_coolTimeMin = baseMinCooldown * (1.0f / (1.0f + _increaseFireRatePowerCount * reduceRatio));
		ReCalculateMaxCoolTime();
	}
}
