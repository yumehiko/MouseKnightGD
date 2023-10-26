using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;
using GTweens.Easings;
using GTweensGodot.Extensions;

namespace MouseKnightGD.InGame.PowerUps;
public partial class PowerUpUi : Control
{
	[Export] private PowerUp[] _powerUps;
	[Export] private PowerUpUiButton[] _powerUpUiButtons;

	public async GDTask Call(CancellationToken ct)
	{
		await Open(ct);
		GD.Print("PowerUpUi.Call await");
		await GDTask.WhenAny(_powerUpUiButtons.Select(x => x.PowerUpTcs.Task));
		await Close(ct);
		GD.Print("PowerUpUi.Call end");
	}

	private async GDTask Open(CancellationToken ct)
	{
		foreach (var button in _powerUpUiButtons)
		{
			var powerUp = GetPowerUp();
			button.Open(powerUp);
		}
		Modulate = Colors.Transparent;
		await this.TweenModulateAlpha(1.0f, 0.6f)
			.SetEasing(Easing.OutQuad)
			.PlayAsync(ct);
	}
	
	private async GDTask Close(CancellationToken ct)
	{
		var closeTasks = new List<GDTask>();
		foreach (var button in _powerUpUiButtons)
		{
			var closeTask = button.Release(ct);
			closeTasks.Add(closeTask);
		}
		await GDTask.WhenAll(closeTasks);
		await this.TweenModulateAlpha(0.0f, 0.6f)
			.SetEasing(Easing.OutQuad)
			.PlayAsync(ct);
	}
	
	private PowerUp GetPowerUp()
	{
		// TODO: 取得できるパワーアップを選び、ランダムに取得する。
		var randomId = GD.RandRange(0, _powerUps.Length - 1);
		GD.Print($"PowerUpUi.GetPowerUp: {randomId}");
		var powerUp = _powerUps[randomId];
		return powerUp;
	}
}
