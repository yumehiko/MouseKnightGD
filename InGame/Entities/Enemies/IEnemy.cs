using Godot;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.Entities.Enemies;

public interface IEnemy : IEntity, IDamageable, IDieable, IRemovable
{
    /// <summary>
    /// この敵の面白さ。Fun値が高いほど強い敵である。
    /// </summary>
    int Fun { get; }
    void Initialize(Vector2 spawnPosition, Hero player);
}