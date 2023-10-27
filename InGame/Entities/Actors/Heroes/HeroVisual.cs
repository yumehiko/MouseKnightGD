using System;
using System.Reactive.Disposables;
using Godot;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame.Entities.Actors.Heroes;

public partial class HeroVisual : Sprite2D
{
    [Export] private AudioStreamPlayer2D _deathSePlayer;
    [Export] private AudioStream _deathSe;
    [Export] private AudioStream _invisibleSe;
    [Export] private AudioStreamPlayer2D _chipSePlayer;
    [Export] private CpuParticles2D _deathParticle;
    [Export] private Color _damageColor0;
    [Export] private Color _damageColor1;

    private CompositeDisposable _disposable;

    public void Initialize(Health health, ChipCollector chipCollector)
    {
        _disposable = new CompositeDisposable();
        health.Current.Subscribe(OnDamage).AddTo(_disposable);
        health.OnDeath.Subscribe(_ => Death()).AddTo(_disposable);
        health.IsInvisible.Subscribe(Invisible).AddTo(_disposable);
        chipCollector.OnCollect.Subscribe(_ => CollectChip()).AddTo(_disposable);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _disposable?.Dispose();
    }

    private void OnDamage(int current)
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
        if (!isInvisible) return;
        _deathSePlayer.Stream = _invisibleSe;
        _deathSePlayer.Play();
        _deathParticle.Amount = 16;
        _deathParticle.Lifetime = 1.0f;
        _deathParticle.Emitting = true;
    }
    
    private void CollectChip()
    {
        _chipSePlayer.Play();
    }

    private void Death()
    {
        Visible = false;
        _deathParticle.Amount = 64;
        _deathParticle.Lifetime = 4.0f;
        _deathParticle.Emitting = true;
        _deathSePlayer.Stream = _deathSe;
        _deathSePlayer.Play();
    }
}