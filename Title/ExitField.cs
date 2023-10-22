using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace MouseKnightGD.Title;

/// <summary>
/// マウスオーバーし続けたら、ゲームを終了する。
/// </summary>
public partial class ExitField : Area2D
{
	[Export] private Sprite2D _fillSprite;
	private Tween _fillScaleTween;
	private bool _isActive;
	private CancellationTokenSource _cts;

	private Subject<Unit> _onExit;
	public IObservable<Unit> OnExit => _onExit;
	
	public void Initialize()
	{
		_isActive = true;
		_onExit = new Subject<Unit>();
	}

	public void Close()
	{
		_isActive = false;
		_onExit.Dispose();
	}

	private void OnMouseEntered()
	{
		if(!_isActive) return;
		_cts = new CancellationTokenSource();
		_fillScaleTween?.Kill();
		_fillScaleTween = GetTree().CreateTween();
		_fillScaleTween.TweenProperty(_fillSprite, "scale", new Vector2(1.0f, 1.0f), 1.5f);
		_fillScaleTween.SetEase(Tween.EaseType.Out);
		_fillScaleTween.Play();
		_ = FillAsync(1.5f);
	}
	
	private void OnMouseExited()
	{
		if(!_isActive) return;
		_cts?.Cancel();
		_fillScaleTween?.Kill();
		_fillScaleTween = GetTree().CreateTween();
		_fillScaleTween.TweenProperty(_fillSprite, "scale", new Vector2(0.0f, 0.0f), 0.75f);
		_fillScaleTween.SetEase(Tween.EaseType.Out);
		_fillScaleTween.Play();
	}

	private async Task FillAsync(float duration)
	{
		_cts = new CancellationTokenSource();
		await Task.Delay(TimeSpan.FromSeconds(duration), _cts.Token);
		_onExit.OnNext(Unit.Default);
	}
}
