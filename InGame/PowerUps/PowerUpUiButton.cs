using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.Core;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PowerUpUiButton : TextureRect
{
	[Export] private AudioStreamPlayer2D _chooseSePlayer;
	private PowerUp _powerUp;
	public GDTaskCompletionSource<PowerUp> PowerUpTcs { get; private set; }
	private readonly float _inActiveAlpha = 0.25f;
	private bool _isPicked;
	
	public void SetPowerUp(PowerUp powerUp)
	{
		_powerUp = powerUp;
		Texture = _powerUp.Describe;
		Modulate = new Color(1.0f, 1.0f, 1.0f, _inActiveAlpha);
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		
		// この時点でマウスがエンター済みかどうかを判定する
		var mousePos = GetGlobalMousePosition();
		var rect = GetGlobalRect();
		var isMouseEntered = rect.HasPoint(mousePos);
		if (isMouseEntered) OnMouseEntered();
	}
	
	public void Activate()
	{
		_isPicked = false;
		PowerUpTcs = new GDTaskCompletionSource<PowerUp>();
		GuiInput += OnGuiInput;
	}
	
	public async GDTask Release(CancellationToken ct)
	{
		GuiInput -= OnGuiInput;
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
		PowerUpTcs.TrySetCanceled(ct);
		if (_isPicked) return;
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 0.0f, 0.4f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		await tween.PlayAsync(ct);
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
		_chooseSePlayer.Play();
	}
	
	private void OnMouseExited()
	{
		if (_isPicked) return;
		Modulate = new Color(1.0f, 1.0f, 1.0f, _inActiveAlpha);
	}
}
