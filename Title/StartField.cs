using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using Fractural.Tasks;
using Godot;

namespace photon.Title;

public partial class StartField : Area2D
{
	[Export] private AudioStreamPlayer _titleSe;
	[Export] private Sprite2D _fillSprite;
	private Tween _fillScaleTween;
	private bool _isActive;
	private CancellationTokenSource _cts;
	
	private Subject<Unit> _onStart;
	public IObservable<Unit> OnStart => _onStart;

	public void Initialize()
	{
		_isActive = true;
		_onStart = new Subject<Unit>();
	}

	public void Close()
	{
		_isActive = false;
		_onStart.Dispose();
	}
	
	private void OnMouseEntered()
	{
		const float duration = 1.0f;
		if(!_isActive) return;
		_titleSe.Play();
		_fillScaleTween?.Kill();
		_fillScaleTween = GetTree().CreateTween();
		_fillScaleTween.TweenProperty(_fillSprite, "scale", new Vector2(1.0f, 1.0f), duration);
		_fillScaleTween.SetEase(Tween.EaseType.Out);
		_fillScaleTween.Play();
		_ = FillAsync(duration);
	}
	
	private void OnMouseExited()
	{
		const float duration = 0.5f;
		if(!_isActive) return;
		_titleSe.Stop();
		_cts?.Cancel();
		_fillScaleTween?.Kill();
		_fillScaleTween = GetTree().CreateTween();
		_fillScaleTween.TweenProperty(_fillSprite, "scale", new Vector2(0.0f, 0.0f), duration);
		_fillScaleTween.SetEase(Tween.EaseType.Out);
		_fillScaleTween.Play();
	}

	private async GDTask FillAsync(float duration)
	{
		_cts = new CancellationTokenSource();
		await GDTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: _cts.Token);
		_onStart.OnNext(Unit.Default);
	}
}
