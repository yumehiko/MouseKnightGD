using System;
using System.Reactive.Disposables;
using Godot;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame.Entities.Actors.Heroes;

public partial class HeroVisual : Sprite2D
{
    [Export] private AudioStreamPlayer2D _deathSound;
    [Export] private CpuParticles2D _deathParticle;
    [Export] private Color _damageColor0;
    [Export] private Color _damageColor1;

    private CompositeDisposable _disposable;

    public void Initialize(Health health)
    {
        _disposable = new CompositeDisposable();
        health.Current.Subscribe(SyncDamageColor).AddTo(_disposable);
        health.OnDeath.Subscribe(_ => Death()).AddTo(_disposable);
        health.IsInvisible.Subscribe(Invisible).AddTo(_disposable);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _disposable?.Dispose();
    }

    private void SyncDamageColor(int current)
    {
        Modulate = current switch
        {
            3 => Colors.White,
            2 => _damageColor0,
            1 => _damageColor1,
            _ => Modulate
        };
    }
    
    private void Invisible(bool isInvisible)
    {
        // 半透明になる
        var invisibleAlpha = new Color(Modulate.R, Modulate.G, Modulate.B, 0.25f);
        var normalAlpha = new Color(Modulate.R, Modulate.G, Modulate.B, 1.0f);
        Modulate = isInvisible ? invisibleAlpha : normalAlpha;
    }

    private void Death()
    {
        Visible = false;
        _deathParticle.Emitting = true;
        _deathSound.Play();
    }
}