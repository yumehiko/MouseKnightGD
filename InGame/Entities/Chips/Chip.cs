using Godot;

namespace MouseKnightGD.InGame.Entities.Chips;

public partial class Chip : RigidBody2D
{
	public int Value = 1;
	
	public void Awake(Vector2 initPosition)
	{
		var x = initPosition.X + GD.Randf() * 32.0f - 16.0f;
		var y = initPosition.Y + GD.Randf() * 32.0f - 16.0f;
		initPosition = new Vector2(x, y);
		Position = initPosition;
		var forceX = GD.Randf() * 2.0f - 1.0f;
		var forceY = GD.Randf() * 2.0f - 1.0f;
		var randomImpulse = new Vector2(forceX, forceY).Normalized() * 8.0f;
		ApplyImpulse(randomImpulse);
		var torque = GD.Randi() % 2 == 0 ? 5120.0f : -5120.0f;
		ApplyTorque(torque);
	}
}
