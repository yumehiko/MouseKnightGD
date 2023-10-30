using System;
using System.Threading;
using Fractural.Tasks;
using Godot;

namespace MouseKnightGD.App;

/// <summary>
/// 画面全体を覆うカーテン。
/// </summary>
public partial class Curtain : Sprite2D
{
	private Tween _tween;
	
	public async GDTask Open(float duration, CancellationToken ct)
	{
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(this, "modulate:a", 0, duration);
		_tween.SetEase(Tween.EaseType.Out);
		_tween.Play();
		await GDTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
	}

	public async GDTask Close(float duration, CancellationToken ct)
	{
		_tween?.Kill();
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(this, "modulate:a", 1, duration);
		_tween.SetEase(Tween.EaseType.Out);
		_tween.Play();
		await GDTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
	}
}
