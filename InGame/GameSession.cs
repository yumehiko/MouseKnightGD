using System;
using System.Threading;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Brains;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using MouseKnightGD.InGame.Entities.Enemies;
using MouseKnightGD.InGame.PowerUps;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame;

public partial class GameSession : Node
{
	[Export] private PlayerBrain _playerBrain;
	[Export] private Hero _playerHero;
	[Export] private StageArea _stageArea;
	[Export] private EnemyFactory _enemyFactory;
	[Export] private PowerUpUi _powerUpUi;
	[Export] private Node2D _projectileRoot;
	
	private PowerUpService _powerUpService;
	private GDTaskCompletionSource<GameSessionResult> _tcs;

	public async GDTask<GameSessionResult> Run(CancellationToken ct)
	{
		GD.Print("GameSession.Run");
		var disposables = new CompositeDisposable();
		_tcs = new GDTaskCompletionSource<GameSessionResult>();
		var gameCts = new CancellationTokenSource();
		_playerHero.Initialize(_playerBrain, _projectileRoot);
		_stageArea.Initialize(_playerHero);
		_enemyFactory.Initialize(_playerHero);
		_powerUpService = new PowerUpService(_playerHero, _powerUpUi, gameCts.Token).AddTo(disposables);
		_playerHero.OnDeath.Subscribe(_ => { }, () => GameOver(gameCts)).AddTo(disposables);
		_ =	MainLoop(gameCts.Token);

		var result = await _tcs.Task.AttachExternalCancellation(ct);
		await GDTask.Delay(TimeSpan.FromSeconds(4.0f), cancellationToken: ct);
		disposables.Dispose();
		return result;
	}
	
	private async GDTask MainLoop(CancellationToken ct)
	{
		_enemyFactory.Create(10);
		while (ct.IsCancellationRequested == false)
		{
			await GDTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: ct);
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
