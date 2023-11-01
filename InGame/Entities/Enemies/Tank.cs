using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.Entities.Enemies;

public partial class Tank : EnemyBase
{
	
	[Export] private DamageArea _damageArea;
	private readonly Random _random = new Random();
	private CancellationTokenSource _cts;
	public override void Initialize(Vector2 spawnPosition, Hero player)
	{
		base.Initialize(spawnPosition, player);
		_damageArea.BodyEntered += GiveDamage;
		_cts = new CancellationTokenSource();
		DamageActionLoop(_cts.Token).Forget();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_damageArea.BodyEntered -= GiveDamage;
		_cts?.Cancel();
		_cts?.Dispose();
	}

	private async GDTask DamageActionLoop(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			await GDTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: ct);
			if (ct.IsCancellationRequested) return;
			await DamageAction(ct);
		}
	}

	private async GDTask DamageAction(CancellationToken ct)
	{
		await _damageArea.Alert(0.75f, ct);
		await GDTask.Delay(TimeSpan.FromSeconds(3.0f), cancellationToken: ct);
		await _damageArea.FadeOut(0.5f, ct);
		_damageArea.ExpandMul(1.5f);
	}
}
