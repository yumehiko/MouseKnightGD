using System;
using System.Reactive;
using System.Reactive.Subjects;
using Fractural.Tasks;
using Godot;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PowerUpUiButton : TextureRect
{
	private PowerUp _powerUp;
	public GDTaskCompletionSource<PowerUp> PowerUpTcs { get; private set; }
	
	public void Register(PowerUp powerUp)
	{
		GD.Print("PowerUpUiButton.Register");
		GuiInput += OnGuiInput;
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		_powerUp = powerUp;
		Texture = _powerUp.Describe;
		PowerUpTcs = new GDTaskCompletionSource<PowerUp>();
	}
	
	public void Release()
	{
		GD.Print("PowerUpUiButton.Release");
		GuiInput -= OnGuiInput;
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
		PowerUpTcs.TrySetCanceled();
	}
	
	private void OnGuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton { Pressed: true })
		{
			PowerUpTcs.TrySetResult(_powerUp);
		}
	}
	
	private void OnMouseEntered()
	{
		Modulate = Colors.White;
	}
	
	private void OnMouseExited()
	{
		Modulate = new Color(1.0f, 1.0f, 1.0f, 0.25f);
	}
}
