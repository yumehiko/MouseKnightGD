using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;
using photon.Core;

namespace photon.InGame.PowerUps;
public partial class PowerUpUi : Control
{
	[Export] private PowerUpUiButton[] _powerUpUiButtons;
	[Export] private AudioStreamPlayer _sePlayer;
	[Export] private AudioStream _levelUpSe;
	[Export] private AudioStream _confirmSe;
	[Export] private AudioStreamPlayer _environmentPlayer;
	[Export] private Texture2D _powerUpStatFrame;
	[Export] private Texture2D _weaponFrame;
	
	private Tween _environmentVolumeTween;
	
	public async GDTask<PowerUpBase> Call(IReadOnlyList<PowerUpBase> powerUps, CancellationToken ct)
	{
		GetTree().Paused = true;
		await Open(powerUps, ct);
		var confirm = await GDTask.WhenAny(_powerUpUiButtons.Select(x => x.PowerUpTcs.Task));
		await Close(ct);
		GetTree().Paused = false;
		return confirm.result;
	}

	private async GDTask Open(IReadOnlyList<PowerUpBase> powerUps, CancellationToken ct)
	{
		_sePlayer.Stream = _levelUpSe;
		_sePlayer.Play();
		_environmentPlayer.StreamPaused = false;
		_environmentVolumeTween?.Kill();
		_environmentVolumeTween = CreateTween();
		_environmentVolumeTween.TweenProperty(_environmentPlayer, "volume_db", 0.0f, 0.8f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		_environmentVolumeTween.Play();
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
		_sePlayer.Stream = _confirmSe;
		_sePlayer.Play();
		_environmentVolumeTween?.Kill();
		_environmentVolumeTween = CreateTween();
		_environmentVolumeTween.TweenProperty(_environmentPlayer, "volume_db", -80.0f, 1.2f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		_environmentVolumeTween.Play();
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
		_environmentPlayer.StreamPaused = true;
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
