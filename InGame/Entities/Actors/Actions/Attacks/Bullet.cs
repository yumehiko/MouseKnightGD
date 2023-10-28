using Godot;
using MouseKnightGD.InGame.Entities.Enemies;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

public partial class Bullet : RigidBody2D
{
	private int _damage;
	public void Shot(Vector2 position, Vector2 direction, float speed, int damage)
	{
		_damage = damage;
		Position = position;
		LinearVelocity = direction * speed;
		ContactMonitor = true;
		MaxContactsReported = 2;
		BodyEntered += OnBodyEntered;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		BodyEntered -= OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is EnemyBase enemy)
		{
			enemy.TakeDamage(_damage);
		}

		QueueFree();
	}
}
