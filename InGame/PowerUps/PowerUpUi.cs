using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;
using MouseKnightGD.Core;

namespace MouseKnightGD.InGame.PowerUps;
public partial class PowerUpUi : Control
{
	[Export] private PowerUpUiButton[] _powerUpUiButtons;
	[Export] private AudioStreamPlayer _confirmSePlayer;
	[Export] private Texture2D _powerUpStatFrame;
	[Export] private Texture2D _weaponFrame;
	public async GDTask<PowerUpBase> Call(IReadOnlyList<PowerUpBase> powerUps, CancellationToken ct)
	{
		GetTree().Paused = true;
		await Open(powerUps, ct);
		GD.Print("PowerUpUi.Call await");
		var confirm = await GDTask.WhenAny(_powerUpUiButtons.Select(x => x.PowerUpTcs.Task));
		await Close(ct);
		GD.Print("PowerUpUi.Call end");
		GetTree().Paused = false;
		return confirm.result;
	}

	private async GDTask Open(IReadOnlyList<PowerUpBase> powerUps, CancellationToken ct)
	{
		for(var i = 0; i < 3; i++)
		{
			var button = _powerUpUiButtons[i];
			var powerUp = powerUps[i];
			var frame = GetFrame(powerUp);
			button.SetPowerUp(powerUp, frame);
		}
		
		Modulate = Colors.Transparent;
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 1.0f, 1.0f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		await tween.PlayAsync(ct);
		foreach (var button in _powerUpUiButtons)
		{
			button.Activate();
		}
	}
	
	private async GDTask Close(CancellationToken ct)
	{
		_confirmSePlayer.Play();
		var closeTasks = new List<GDTask>();
		foreach (var button in _powerUpUiButtons)
		{
			var closeTask = button.Release(ct);
			closeTasks.Add(closeTask);
		}
		await GDTask.WhenAll(closeTasks);
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 0.0f, 0.6f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		await tween.PlayAsync(ct);
	}

	private Texture2D GetFrame(PowerUpBase powerUp)
	{
		var frame = powerUp switch
		{
			PowerUpStats _ => _powerUpStatFrame,
			WeaponPack _ => _weaponFrame,
			_ => throw new ArgumentOutOfRangeException(nameof(powerUp))
		};
		return frame;
	}
}
