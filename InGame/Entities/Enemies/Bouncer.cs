using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Hero = MouseKnightGD.InGame.Entities.Actors.Heroes.Hero;

namespace MouseKnightGD.InGame.Entities.Enemies;

public partial class Bouncer : EnemyBase
{
	[Export] private CollisionShape2D _damageShape2D;
	[Export] private Sprite2D _damageAreaVisual;
	private readonly Random _random = new Random();
	private CancellationTokenSource _cts;
	public override void Initialize(Vector2 spawnPosition, Hero player)
	{
		base.Initialize(spawnPosition, player);
		_cts = new CancellationTokenSource();
		var isRight = _random.Next(0, 2) == 0;
		var isUp = _random.Next(0, 2) == 0;
		var x = isRight ? 160 : -160;
		var y = isUp ? 120 : -120;
		LinearVelocity = new Vector2(x, y);
		// トルクも与える
		ApplyTorque(x * 40);
		_ = DamageActionLoop(_cts.Token);
	}
	private async Task DamageActionLoop(CancellationToken ct)
	{
		var damageArea = new DamageAreaEffect(_damageAreaVisual);
		while (!ct.IsCancellationRequested)
		{
			await Task.Delay(TimeSpan.FromSeconds(2.0f), ct);
			await DamageAction(damageArea, ct);
		}
	}

	private async Task DamageAction(DamageAreaEffect damageAreaEffect, CancellationToken ct)
	{
		try
		{
			var tween = CreateTween();
			await damageAreaEffect.Alert(tween, 1.5f, ct);
			_damageShape2D.Disabled = false;
			await Task.Delay(TimeSpan.FromSeconds(3.0f), ct);
			_damageShape2D.Disabled = true;
			tween = CreateTween();
			await damageAreaEffect.FadeOut(tween, 0.5f, ct);
		}
		finally
		{
			_damageShape2D.Disabled = true;
		}
	}
}
