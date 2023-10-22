using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Hero = MouseKnightGD.InGame.Entities.Actors.Heroes.Hero;

namespace MouseKnightGD.InGame.Entities.Enemies;

public partial class Bouncer : EnemyBase
{
	[Export] private DamageArea _damageArea;
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

	public override void _ExitTree()
	{
		base._ExitTree();
		_cts?.Cancel();
		_cts?.Dispose();
	}

	private async Task DamageActionLoop(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			await Task.Delay(TimeSpan.FromSeconds(2.0f), ct);
			await DamageAction(ct);
		}
	}

	private async Task DamageAction(CancellationToken ct)
	{
		await _damageArea.Alert(0.5f, ct);
		await Task.Delay(TimeSpan.FromSeconds(3.0f), ct);
		await _damageArea.FadeOut(0.5f, ct);
	}
}
