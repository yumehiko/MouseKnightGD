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
	[Export] private PowerUp[] _powerUps;
	[Export] private PowerUpUiButton[] _powerUpUiButtons;
	[Export] private AudioStreamPlayer _confirmSePlayer;

	public async GDTask Call(CancellationToken ct)
	{
		GetTree().Paused = true;
		await Open(ct);
		GD.Print("PowerUpUi.Call await");
		await GDTask.WhenAny(_powerUpUiButtons.Select(x => x.PowerUpTcs.Task));
		await Close(ct);
		GD.Print("PowerUpUi.Call end");
		GetTree().Paused = false;
	}

	private async GDTask Open(CancellationToken ct)
	{
		foreach (var button in _powerUpUiButtons)
		{
			var powerUp = GetPowerUp();
			button.SetPowerUp(powerUp);
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
	
	private PowerUp GetPowerUp()
	{
		// TODO: 取得できるパワーアップを選び、ランダムに取得する。
		var randomId = GD.RandRange(0, _powerUps.Length - 1);
		var powerUp = _powerUps[randomId];
		return powerUp;
	}
}
