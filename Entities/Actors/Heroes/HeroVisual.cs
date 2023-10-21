using Godot;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.Entities.Actors.Heroes;

public partial class HeroVisual : Sprite2D
{
    [Export] private CpuParticles2D _deathParticle;
    [Export] private Color _damageColor0;
    [Export] private Color _damageColor1;

    private CompositeDisposable _disposable;

    public void Initialize(Health health)
    {
        _disposable = new CompositeDisposable();
        health.Current.Subscribe(SyncDamageColor).AddTo(_disposable);
        health.OnDeath.Subscribe(_ => Death()).AddTo(_disposable);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _disposable?.Dispose();
    }

    public void SyncDamageColor(int current)
    {
        Modulate = current switch
        {
            3 => Colors.White,
            2 => _damageColor0,
            1 => _damageColor1,
            _ => Modulate
        };
    }

    private void Death()
    {
        this.Visible = false;
        _deathParticle.Emitting = true;
    }
}