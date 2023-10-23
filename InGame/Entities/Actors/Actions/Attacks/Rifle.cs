using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

public partial class Rifle : Node2D
{
	[Export] private PackedScene _bulletPack;
	private float _coolTimeMax = 1.5f;
	private float _coolTimeMin = 0.5f;
	private float _coolTime;
	private List<Vector2> _barrelAngles;
	private Node2D _projectileRoot;
	private CooldownTimer _timer;
	private CompositeDisposable _disposable;
	private CancellationTokenSource _cts;

	public void Initialize(Hero hero)
	{
		_timer = new CooldownTimer();
		_disposable = new CompositeDisposable();
		_cts = new CancellationTokenSource();
		_projectileRoot = hero.ProjectileRoot;
		_barrelAngles = new List<Vector2>();
		// 上下左右にバレルアングルを設定する
		_barrelAngles.Add(Vector2.Up);
		_barrelAngles.Add(Vector2.Down);
		_barrelAngles.Add(Vector2.Left);
		_barrelAngles.Add(Vector2.Right);
		
		_coolTime = _coolTimeMax;
		
		hero.Brain.LeftTrigger
			.Where(_ => !hero.IsDead)
			.Where(isOn => isOn)
			.Where(_ => !_timer.InCooldown.Value)
			.Subscribe(_ => Shot()).AddTo(_disposable);
		
		hero.Brain.LeftTrigger
			.Where(_ => !hero.IsDead)
			.Where(isOn => !isOn)
			.Subscribe(_ => ReleaseTrigger()).AddTo(_disposable);
		
		_timer.InCooldown
			.Where(inCd => !inCd)
			.Where(_ => hero.Brain.LeftTrigger.Value)
			.Subscribe(_ => Shot()).AddTo(_disposable);
	}

	public override void _ExitTree()
	{
		_disposable?.Dispose();
		base._ExitTree();
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
			instance.Shot(GlobalPosition, barrelAngle, 640.0f);
		}
		_timer.CountAsync(_coolTime, _cts.Token).Forget();
		_coolTime = Mathf.Max(_coolTimeMin, _coolTime - 0.25f);
	}
}
