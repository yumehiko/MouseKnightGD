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
/// スピア
/// マウスクリックを離したとき、最も近い敵を刺突する範囲攻撃。
/// </summary>
public partial class Spear : AttackBase
{
	[Export] private Area2D _area;
	[Export] private Sprite2D _visual;
	private CooldownTimer _timer;
	private CancellationTokenSource _cts;
	private CompositeDisposable _disposable;
	private bool _isAttackTriggered;
	private Vector2 _slashPoint;
	private int _damage = 6;

	public override void Initialize(WeaponHand weaponHand)
	{
		_timer = new CooldownTimer();
		_cts = new CancellationTokenSource();
		_disposable = new CompositeDisposable();
		weaponHand.LeftTrigger
			.Where(_ => !weaponHand.IsDead)
			.Where(isOn => !isOn)
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
		const float cooldown = 0.6f;
		if(!_timer.InCooldown.Value) _isAttackTriggered = true;
		_timer.CountAsync(cooldown, ct).Forget();
	}

	private void GiveDamageToArea()
	{
		if (!_isAttackTriggered) return;
		PierceAnimation();
		// TODO: 最も近い敵を取得し、その方向に_areaを向ける
		// 範囲内の敵にダメージを与える
		var bodies = _area.GetOverlappingBodies();
		var enemies = bodies.OfType<IEnemy>();
		foreach (var enemy in enemies)
		{
			enemy.TakeDamage(_damage);
		}
		_isAttackTriggered = false;
	}
	
	private void PierceAnimation()
	{
		// _visualを表示する
		_visual.Show();
		_visual.Modulate = Colors.White;
		// _visualをTweenで徐々にフェードアウトする
		var tween = CreateTween();
		tween.TweenProperty(_visual, "modulate:a", 0, 0.2f);
		tween.Play();
	}
}
