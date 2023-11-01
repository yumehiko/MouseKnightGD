using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using photon.InGame;
using photon.Title;

namespace photon.App;

/// <summary>
/// このアプリの実行セッション。
/// </summary>
public partial class AppSession : Node
{
	[Export] private Curtain _curtain;
	[Export] private MusicPlayer _musicPlayer;
	[Export] private LoadableSession _gameSessionPack;
	[Export] private LoadableSession _titleSessionPack;

	private CancellationTokenSource _cts;
	
	public override void _Ready()
	{
		base._Ready();
		_ = Begin();
	}

	public override void _ExitTree()
	{
		_cts?.Cancel();
		base._ExitTree();
	}

	private async GDTask Begin()
	{
		_cts = new CancellationTokenSource();
		while (_cts.IsCancellationRequested == false)
		{
			var titleResult = await CallTitle(_cts.Token);
			if (titleResult.Type == TitleSessionResult.ResultType.Exit)
			{
				GetTree().Quit();
				return;
			}
			var gameResult = await CallGame(_cts.Token);
			var saveData = new UserSaveData();
			saveData.WriteHighScore(gameResult.Score);
		}
	}

	private async GDTask<TitleSessionResult> CallTitle(CancellationToken ct)
	{
		var titleSession = _titleSessionPack.Load<TitleSession>(_musicPlayer);
		AddChild(titleSession);
		await GDTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: ct);
		await _curtain.Open(1.0f, ct);
		var result = await titleSession.Run(ct);
		var musicFadeoutTask = _musicPlayer.Fadeout(0.5f, ct);
		var curtainTask = _curtain.Close(0.5f, ct);
		await GDTask.WhenAll(musicFadeoutTask, curtainTask);
		titleSession.QueueFree();
		return result;
	}

	private async GDTask<GameSessionResult> CallGame(CancellationToken ct)
	{
		var gameSession = _gameSessionPack.Load<GameSession>(_musicPlayer);
		AddChild(gameSession);
		await GDTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: ct);
		await _curtain.Open(1.0f, ct);
		var result = await gameSession.Run(_musicPlayer, ct);
		var musicFadeoutTask = _musicPlayer.Fadeout(0.5f, ct);
		var curtainTask = _curtain.Close(0.5f, ct);
		await GDTask.WhenAll(musicFadeoutTask, curtainTask);
		gameSession.QueueFree();
		return result;
	}
}
