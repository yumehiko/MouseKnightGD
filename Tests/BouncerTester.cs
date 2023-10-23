using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.InGame.Entities.Enemies;

namespace MouseKnightGD.Tests;

public partial class BouncerTester : Node
{
	[Export] private PackedScene _bouncerPack;
	private Bouncer _bouncer;
	
	public override void _Ready()
	{
		_ = Test();
	}
	
	private async GDTask Test()
	{
		for (var i = 0; i < 10; i++)
		{
			_bouncer = _bouncerPack.Instantiate<Bouncer>();
			AddChild(_bouncer);
			_bouncer.Initialize(Vector2.Zero, null);
			await GDTask.Delay(1000);
		}
	}
}
