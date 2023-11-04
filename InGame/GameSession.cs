using System;
using System.Reactive.Linq;
using System.Threading;
using Fractural.Tasks;
using Godot;
using photon.App;
using photon.InGame.Entities.Actors.Brains;
using photon.InGame.Entities.Actors.Heroes;
using photon.InGame.Entities.Chips;
using photon.InGame.Entities.Enemies;
using photon.InGame.PowerUps;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace photon.InGame;

public partial class GameSession : Node
{
	[Export] private AudioStream _gameMusic;
	[Export] private PlayerBrain _playerBrain;
	[Export] private Hero _playerHero;
	[Export] private StageArea _stageArea;
	[Export] private ChipFactory _chipFactory;
	[Export] private EnemyFactory _enemyFactory;
	[Export] private PowerUpUi _powerUpUi;
	[Export] private Node2D _projectileRoot;
	[Export] private PowerUpService _powerUpService;
	[Export] private ScoreLabel _scoreLabel;
	[Export] private GameProgressBar _gameProgressBar;

	private CancellationTokenSource _sessionCts;
	private CancellationTokenSource _loopCts;
	private GDTaskCompletionSource<GameSessionResult> _tcs;
	private AiDirector _aiDirector;

	public override void _ExitTree()
	{
		_loopCts?.Cancel();
		_loopCts?.Dispose();
		_sessionCts?.Cancel();
		_sessionCts?.Dispose();
		GD.Print("GameSession._ExitTree");
		base._ExitTree();
	}

	public async GDTask<GameSessionResult> Run(MusicPlayer musicPlayer, CancellationToken ct)
	{
		GD.Print("GameSession.Run");
		var disposables = new CompositeDisposable();
		_tcs = new GDTaskCompletionSource<GameSessionResult>();
		_loopCts = new CancellationTokenSource();
		_sessionCts = new CancellationTokenSource();
		_playerHero.Initialize(_playerBrain, _projectileRoot);
		_stageArea.Initialize(_playerHero);
		_enemyFactory.Initialize(_playerHero);
		_chipFactory.Initialize(_stageArea);
		_powerUpService.Initialize(_playerHero, _powerUpUi, _loopCts.Token);
		_scoreLabel.Initialize(_playerHero);
		_playerHero.OnDeath.Subscribe(_ => { }, () => GameOver(_loopCts).Forget()).AddTo(disposables);
		_aiDirector = new AiDirector(_enemyFactory, 2.05);
		// まず、プレイヤーの最初のレベルアップを待機する。
		await ReadyPart(_loopCts.Token);
		
		// ゲームのメインループ開始
		musicPlayer.SetMusic(_gameMusic);
		ProgressCount(_loopCts).Forget();
		MainLoop(_loopCts.Token).Forget();
		var result = await _tcs.Task.AttachExternalCancellation(ct);
		disposables.Dispose();
		return result;
	}

	private async GDTask ReadyPart(CancellationToken ct)
	{
		for(var i = 0; i < 5; i++)
		{
			_chipFactory.CreateAtRandom();
		}
		await _powerUpService.OnLevelUp.FirstAsync().ToGDTask(cancellationToken: ct);
	}
	
	private async GDTask MainLoop(CancellationToken ct)
	{
		while (ct.IsCancellationRequested == false)
		{
			await GDTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: ct);
			_aiDirector.OnBoreTick(ct);
		}
	}

	private async GDTask ProgressCount(CancellationTokenSource loopCts)
	{
		const float musicLength = 481.0f;
		await _gameProgressBar.StartAsync(musicLength, loopCts.Token);
		await BeatGame(loopCts);
	}

	private async GDTask GameOver(CancellationTokenSource loopCts)
	{
		GD.Print("GameSession.GameOver");
		loopCts?.Cancel();
		loopCts?.Dispose();
		_loopCts = null;
		var result = new GameSessionResult(_playerHero.Score.Value);
		await GDTask.Delay(TimeSpan.FromSeconds(4.0f), cancellationToken: _sessionCts.Token);
		_tcs.TrySetResult(result);
	}

	private async GDTask BeatGame(CancellationTokenSource loopCts)
	{
		GD.Print("GameSession.BeatGame");
		loopCts?.Cancel();
		loopCts?.Dispose();
		_loopCts = null;
		_powerUpService.UnRegister(); // レベルアップ機能を停止。
		_enemyFactory.KillAll();
		await GDTask.Delay(TimeSpan.FromSeconds(8.0f), cancellationToken: _sessionCts.Token);
		_playerHero.Die();
	}
}
