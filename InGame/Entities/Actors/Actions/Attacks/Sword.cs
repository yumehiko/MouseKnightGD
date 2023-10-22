using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
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
public partial class Sword : Area2D, IAttack
{
	[Export] private Sprite2D _visual;
	private CooldownTimer _timer;
	private CancellationTokenSource _cts;
	private CompositeDisposable _disposable;
	private bool _isAttackTriggered;
	private Vector2 _slashPoint;

	public void Initialize(Hero hero)
	{
		_timer = new CooldownTimer();
		_cts = new CancellationTokenSource();
		Position = new Vector2(0, -32);
		_disposable = new CompositeDisposable();
		hero.Brain.LeftTrigger
			.Where(_ => !hero.IsDead)
			.Where(isOn => isOn)
			.Subscribe(x => Attack(hero.Position, _cts.Token)).AddTo(_disposable);
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

	private void Attack(Vector2 point, CancellationToken ct)
	{
		GD.Print("Sword.Attack");
		const float cooldown = 0.5f;
		if(_timer.IsCompleted) _isAttackTriggered = true;
		_timer.CountAsync(cooldown, ct).Forget();
	}

	private void GiveDamageToArea()
	{
		if (!_isAttackTriggered) return;
		SlashAnimation();
		// 範囲内の敵にダメージを与える
		var bodies = GetOverlappingBodies();
		var enemies = bodies.OfType<IEnemy>();
		foreach (var enemy in enemies)
		{
			enemy.TakeDamage(1);
		}
		_isAttackTriggered = false;
	}
	
	private void SlashAnimation()
	{
		// _visualを表示する
		_visual.Show();
		_visual.Modulate = new Color(1, 1, 1, 1);
		// _visualをTweenで徐々にフェードアウトする
		var tween = CreateTween();
		tween.TweenProperty(_visual, "modulate:a", 0, 0.2f);
		tween.Play();
	}
}
