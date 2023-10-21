using Godot;
using MouseKnightGD.Entities.Actors.Heroes;

namespace MouseKnightGD.Entities.Chips;

public partial class Chip : RigidBody2D
{
	[Export] public int Value;
	
	public void Awake(Vector2 initPosition)
	{
		Position = initPosition;
		var x = GD.Randf() * 2.0f - 1.0f;
		var y = GD.Randf() * 2.0f - 1.0f;
		var randomImpulse = new Vector2(x, y).Normalized() * 8.0f;
		ApplyImpulse(randomImpulse);
		var torque = GD.Randi() % 2 == 0 ? 5120.0f : -5120.0f;
		ApplyTorque(torque);
	}
}
