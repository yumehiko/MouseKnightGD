using Godot;
using MouseKnightGD.Entities.Actors;
using MouseKnightGD.Entities.Actors.Brains;

namespace MouseKnightGD.Sessions;

public partial class GameSession : Node
{
	[Export] private PlayerBrain _playerBrain;
	[Export] private Hero _playerHero;
	public override void _Ready()
	{
		GD.Print("GameSession Ready");
		_playerHero.Initialize(_playerBrain);
		base._Ready();
	}
}
