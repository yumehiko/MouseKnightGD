using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.Entities.Enemies;

/// <summary>
/// ストーカー。プレイヤーへ常に近づいていく。
/// </summary>
public partial class Stalker : EnemyBase
{
    [Export] private DamageArea _damageArea;
    private float _speed = 300;
    private Node2D _target;
    private CancellationTokenSource _cts;
    public override void Initialize(Vector2 spawnPosition, Hero player)
    {
        base.Initialize(spawnPosition, player);
        _damageArea.BodyEntered += GiveDamage;
        _cts = new CancellationTokenSource();
        _target = player;
        DamageActionLoop(_cts.Token).Forget();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var isTooClose = Position.DistanceTo(_target.Position) < 10;
        var direction = Position.DirectionTo(_target.GlobalPosition);
        LinearVelocity = isTooClose ? Vector2.Zero : direction * _speed;
        Rotation = direction.Angle();
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
            await GDTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: ct);
            if (ct.IsCancellationRequested) return;
            await DamageAction(ct);
        }
    }

    private async GDTask DamageAction(CancellationToken ct)
    {
        await _damageArea.Alert(0.5f, ct);
        await GDTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: ct);
        await _damageArea.FadeOut(0.5f, ct);
    }
}