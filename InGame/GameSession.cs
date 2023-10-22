using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Brains;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using MouseKnightGD.InGame.Entities.Enemies;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame;

public partial class GameSession : Node
{
	[Export] private PlayerBrain _playerBrain;
	[Export] private Hero _playerHero;
	[Export] private StageArea _stageArea;
	[Export] private EnemyFactory _enemyFactory;
	[Export] private AttackFactory _attackFactory;

	private TaskCompletionSource<GameSessionResult> _tcs;

	public async Task<GameSessionResult> Run(CancellationToken ct)
	{
		GD.Print("GameSession.Run");
		var disposables = new CompositeDisposable();
		_tcs = new TaskCompletionSource<GameSessionResult>();
		var gameCts = new CancellationTokenSource();
		_playerHero.Initialize(_playerBrain);
		_stageArea.Initialize(_playerHero);
		_enemyFactory.Initialize(_playerHero);
		_attackFactory.Initialize(_playerHero);
		_playerHero.OnDeath.Subscribe(_ => { }, () => GameOver(gameCts)).AddTo(disposables);
		_ =	MainLoop(gameCts.Token);
		
		var result = await _tcs.Task;
		await Task.Delay(TimeSpan.FromSeconds(3.0f), ct);
		disposables.Dispose();
		return result;
	}
	
	private async Task MainLoop(CancellationToken ct)
	{
		_enemyFactory.Create(10);
		while (ct.IsCancellationRequested == false)
		{
			await Task.Delay(TimeSpan.FromSeconds(1.0f), ct);
			_enemyFactory.Create(10);
		}
	}

	private void GameOver(CancellationTokenSource gameCts)
	{
		gameCts.Cancel();
		var result = new GameSessionResult(100);
		_tcs.TrySetResult(result);
	}
}
