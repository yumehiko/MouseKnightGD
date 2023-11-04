using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.Entities.Enemies;

public partial class Paddle : EnemyBase
{
	[Export] private DamageArea _damageArea;
	private readonly Random _random = new Random();
	private CancellationTokenSource _cts;
	public override void Initialize(Vector2 spawnPosition, Hero player)
	{
		IsBulletProof = true;
		base.Initialize(spawnPosition, player);
		_damageArea.BodyEntered += GiveDamage;
		_cts = new CancellationTokenSource();
		var y = _random.Next(0, 2) == 0 ? 100 : -100;
		LinearVelocity = new Vector2(0, y);
		EnterDamageMode(_cts.Token).Forget();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_damageArea.BodyEntered -= GiveDamage;
		_cts?.Cancel();
		_cts?.Dispose();
	}

	private async GDTask EnterDamageMode(CancellationToken ct)
	{
		await GDTask.Delay(TimeSpan.FromSeconds(3.0f), cancellationToken: ct);
		await _damageArea.Alert(1.0f, ct);
	}
}
