using Godot;
using MouseKnightGD.Entities.Actors.Heroes;

namespace MouseKnightGD.Entities.Enemies;

public interface IEnemy : IEntity, IDamageable, IDieable, IRemovable
{
    /// <summary>
    /// この敵の面白さ。Fun値が高いほど強い敵である。
    /// </summary>
    double Fun { get; }
    void Initialize(Vector2 spawnPosition, Hero player);
}