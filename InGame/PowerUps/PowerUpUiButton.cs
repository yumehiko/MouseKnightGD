using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PowerUpUiButton : TextureRect
{
	[Export] 
	private PowerUp _powerUp;
	public GDTaskCompletionSource<PowerUp> PowerUpTcs { get; private set; }
	private readonly float _inActiveAlpha = 0.25f;
	private bool _isPicked;
	
	public void Open(PowerUp powerUp)
	{
		_isPicked = false;
		_powerUp = powerUp;
		Texture = _powerUp.Describe;
		PowerUpTcs = new GDTaskCompletionSource<PowerUp>();
		Modulate = new Color(1.0f, 1.0f, 1.0f, _inActiveAlpha);
		GuiInput += OnGuiInput;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}
	
	public async GDTask Release(CancellationToken ct)
	{
		GuiInput -= OnGuiInput;
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
		PowerUpTcs.TrySetCanceled(ct);
		if (_isPicked) return;
		await this.TweenModulateAlpha(0.0f, 0.3f)
			.SetEasing(Easing.OutQuad)
			.PlayAsync(ct);
	}
	
	private void OnGuiInput(InputEvent @event)
	{
		if (@event is not InputEventMouseButton { Pressed: true }) return;
		_isPicked = true;
		PowerUpTcs.TrySetResult(_powerUp);
	}
	
	private void OnMouseEntered()
	{
		Modulate = Colors.White;
	}
	
	private void OnMouseExited()
	{
		if (_isPicked) return;
		Modulate = new Color(1.0f, 1.0f, 1.0f, _inActiveAlpha);
	}
}
