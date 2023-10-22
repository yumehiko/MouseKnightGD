using System.Threading;
using System.Threading.Tasks;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;
using MouseKnightGD.InGame.Entities.Actors.Brains;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using MouseKnightGD.InGame.Entities.Enemies;

namespace MouseKnightGD.InGame;

public partial class GameSession : Node
{
	[Export] private PlayerBrain _playerBrain;
	[Export] private Hero _playerHero;
	[Export] private StageArea _stageArea;
	[Export] private EnemyFactory _enemyFactory;
	[Export] private AttackFactory _attackFactory;
	public override void _Ready()
	{
		GD.Print("GameSession Ready");
		_playerHero.Initialize(_playerBrain);
		_stageArea.Initialize(_playerHero);
		_enemyFactory.Initialize(_playerHero);
		_attackFactory.Initialize(_playerHero);
		base._Ready();
		var cts = new CancellationTokenSource();
		_ = MainLoop(cts.Token);
	}
	
	private async Task MainLoop(CancellationToken ct)
	{
		while (ct.IsCancellationRequested == false)
		{
			await Task.Delay(1000, ct);
			_enemyFactory.Create(10);
		}
	}
}
