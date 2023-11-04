using Godot;
using photon.InGame.Entities.Enemies;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

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
			var isBulletProof = enemy.IsBulletProof;
			var damage = isBulletProof ? 1 : _damage;
			enemy.TakeDamage(damage);
			
			// この時点でダメージが最低値なら、削除。
			if (_damage <= 1)
			{
				QueueFree();
				return;
			}
			
			// 反射後のダメージは1固定
			_damage = 1;
			
			// 相手が防弾なら、自分は消えない
			if (isBulletProof) return;
		}

		QueueFree();
	}
}
