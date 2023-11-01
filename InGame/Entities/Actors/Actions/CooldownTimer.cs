using System;
using System.Threading;
using Fractural.Tasks;
using Reactive.Bindings;

namespace photon.InGame.Entities.Actors.Actions;

/// <summary>
/// クールダウン処理のためのタイマー。
/// </summary>
public class CooldownTimer
{
	private GDTaskCompletionSource<bool> _tcs;
	private readonly ReactivePropertySlim<bool> _inCooldown;
	public IReadOnlyReactiveProperty<bool> InCooldown => _inCooldown;

	public CooldownTimer(bool initCompleted = true)
	{
		_inCooldown = new ReactivePropertySlim<bool>(!initCompleted);
	}
	
	/// <summary>
	/// タイマーカウントを開始する。
	/// すでにタイマーが動いている場合は、そのタイマーが終了するまで待機する。
	/// </summary>
	public async GDTask CountAsync(float duration, CancellationToken ct)
	{
		if (_tcs != null)
		{
			await _tcs.Task.AttachExternalCancellation(ct);
			_tcs = null;
			return;
		}
		
		_tcs = new GDTaskCompletionSource<bool>();
		Count(duration).Forget();
		await _tcs.Task.AttachExternalCancellation(ct);
		_tcs = null;
	}

	private async GDTask Count(float duration)
	{
		_inCooldown.Value = true;
		await GDTask.Delay(TimeSpan.FromSeconds(duration));
		_tcs.TrySetResult(true);
		_inCooldown.Value = false;
	}
}
