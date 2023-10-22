using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace MouseKnightGD.App;

/// <summary>
/// 画面全体を覆うカーテン。
/// </summary>
public partial class Curtain : Sprite2D
{
    private Tween _tween;
    
    public async Task Open(float duration, CancellationToken ct)
    {
        _tween?.Kill();
        _tween = GetTree().CreateTween();
        _tween.TweenProperty(this, "modulate.a", 1.0f, duration);
        _tween.SetEase(Tween.EaseType.Out);
        _tween.Play();
        await Task.Delay(TimeSpan.FromSeconds(duration), ct);
    }

    public async Task Close(float duration, CancellationToken ct)
    {
        _tween?.Kill();
        _tween = GetTree().CreateTween();
        _tween.TweenProperty(this, "modulate.a", 0.0f, duration);
        _tween.SetEase(Tween.EaseType.Out);
        _tween.Play();
        await Task.Delay(TimeSpan.FromSeconds(duration), ct);
    }
}