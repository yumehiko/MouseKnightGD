using System;
using System.Threading;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;

namespace MouseKnightGD.InGame.Entities.Actors.Actions;

/// <summary>
/// クールダウン処理のためのタイマー。
/// </summary>
public partial class CooldownTimer
{
	private TaskCompletionSource<bool> _tcs;
	public bool IsCompleted { get; private set; }

	public CooldownTimer(bool initCompleted = true)
	{
		IsCompleted = initCompleted;
	}
	

	/// <summary>
	/// タイマーカウントを実行する。
	/// すでにカウントが完了していた場合、即座に完了を返す。
	/// すでにタイマーが動いている場合は、そのタイマーが終了するまで待機する。
	/// </summary>
	public async GDTask CountAsync(float duration, CancellationToken ct)
	{
		if (_tcs != null)
		{
			await _tcs.Task;
			_tcs = null;
			IsCompleted = true;
			return;
		}
		
		IsCompleted = false;
		_tcs = new TaskCompletionSource<bool>();
		ct.Register(() => _tcs.TrySetCanceled());
		Count(duration).Forget();
		await _tcs.Task;
		_tcs = null;
		IsCompleted = true;
	}

	private async GDTask Count(float duration)
	{
		await GDTask.Delay(TimeSpan.FromSeconds(duration));
		_tcs.TrySetResult(true);
		IsCompleted = true;
	}
}
