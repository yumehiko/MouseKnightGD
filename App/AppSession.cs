using System;
using System.Threading;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.InGame;
using MouseKnightGD.Title;

namespace MouseKnightGD.App;

/// <summary>
/// このアプリの実行セッション。
/// </summary>
public partial class AppSession : Node
{
	[Export] private Curtain _curtain;
	[Export] private PackedScene _titleSessionPack;
	[Export] private PackedScene _gameSessionPack;

	private CancellationTokenSource _cts;
	
	public override void _Ready()
	{
		base._Ready();
		_ = Begin();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_cts?.Cancel();
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
			GD.Print(gameResult.Score);
		}
	}

	private async GDTask<TitleSessionResult> CallTitle(CancellationToken ct)
	{
		var titleSession = _titleSessionPack.Instantiate<TitleSession>();
		AddChild(titleSession);
		await GDTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: ct);
		await _curtain.Open(1.0f, ct);
		var result = await titleSession.Run(ct);
		await _curtain.Close(0.5f, ct);
		titleSession.QueueFree();
		return result;
	}

	private async GDTask<GameSessionResult> CallGame(CancellationToken ct)
	{
		var gameSession = _gameSessionPack.Instantiate<GameSession>();
		AddChild(gameSession);
		await GDTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: ct);
		await _curtain.Open(1.0f, ct);
		var result = await gameSession.Run(ct);
		await _curtain.Close(0.5f, ct);
		gameSession.QueueFree();
		return result;
	}
}
