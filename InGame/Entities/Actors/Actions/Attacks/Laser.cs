using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot.Extensions;
using photon.InGame.Entities.Actors.Heroes;
using photon.InGame.Entities.Enemies;
using Reactive.Bindings.Extensions;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

public partial class Laser : AttackBase
{
	[Export] private PackedScene _barrelPack;
	[Export] private Node2D _pivot;
	[Export] private AudioStreamPlayer2D _fireSound;
	private List<LaserBarrel> _barrels;
	private IWeaponHand _weaponHand;
	private CancellationTokenSource _cts;
	private CompositeDisposable _disposable;
	private CooldownTimer _timer;
	private bool _isCharging;
	private bool _isFiring;
	private int _damage = 32;
	private int _chargeCount = 0;
	private float _chargeSpan = 0.3f;
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		// _pivotを回転する。6.25秒で1回転
		var radians = (float) (Math.PI * 0.32 * delta);
		_pivot.Rotate(radians);
	}
	public override void Initialize(IWeaponHand weaponHand)
	{
		_weaponHand = weaponHand;
		_barrels = new List<LaserBarrel>();
		_cts = new CancellationTokenSource();
		_disposable = new CompositeDisposable();
		_timer = new CooldownTimer();
		AddBarrel();
		
		_timer.InCooldown
			.Where(_ => !weaponHand.IsDead)
			.Subscribe(SetCooldownColor).AddTo(_disposable);
		
		// クールダウンが終わったとき、チャージを開始する。
		_timer.InCooldown
			.Where(_ => !weaponHand.IsDead)
			.Where(inCd => !inCd)
			.Subscribe(_ => StartCharge(_cts.Token).Forget()).AddTo(_disposable);
		
		// トリガーを離したとき、レーザーを放つ。
		weaponHand.LeftTrigger
			.Where(_ => !weaponHand.IsDead)
			.Where(isOn => !isOn)
			.Where(_ => _isCharging)
			.Subscribe(_ => Fire(_cts.Token).Forget()).AddTo(_disposable);
		
		weaponHand.OnDeath.Subscribe(_ => { }, Disable);
	}

	private void Disable()
	{
		Hide();
		_cts?.Cancel();
		_cts?.Dispose();
		_disposable?.Dispose();
	}

	private async GDTask StartCharge(CancellationToken ct)
	{
		_isCharging = true;
		await GDTask.Delay(TimeSpan.FromSeconds(_chargeSpan), cancellationToken: ct);

		// 0.5秒ごとにダメージを1増やす
		while (_isCharging && !ct.IsCancellationRequested && _chargeCount < 60)
		{
			_chargeCount++;
			foreach (var barrel in _barrels)
			{
				barrel.OnChargeTick(_chargeCount);
			}
			await GDTask.Delay(TimeSpan.FromSeconds(_chargeSpan), cancellationToken: ct);
		}
	}
	
	private async GDTask Fire(CancellationToken ct)
	{
		_isCharging = false;
		_timer.CountAsync(3.0f, ct).Forget();
		var tasks = _barrels.Select(barrel => barrel.Fire(_damage, _chargeCount, ct)).ToList();
		_chargeCount = 0;
		_fireSound.Play();
		await GDTask.WhenAll(tasks);
	}

	private void SetCooldownColor(bool isCd)
	{
		foreach (var barrel in _barrels)
		{
			barrel.SetCooldownColor(isCd);
		}
	}

	public void AddBarrel()
	{
		var barrel = _barrelPack.Instantiate<LaserBarrel>();
		_pivot.AddChild(barrel);
		barrel.Initialize(_weaponHand);
		barrel.SetCooldownColor(_timer.InCooldown.Value);
		_barrels.Add(barrel);
	}

	public void Accel()
	{
		_chargeSpan *= 0.8f;
	}
}
