using Godot;

namespace MouseKnightGD.Entities.Enemies;

public partial class Bouncer : RigidBody2D, IEnemy
{
    public void TakeDamage(int amount)
    {
        // 自分を破壊
        QueueFree();
    }
}