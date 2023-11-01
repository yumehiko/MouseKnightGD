using Godot;

namespace photon.InGame.Entities.Enemies;

public partial class EnemyPack : Resource
{
    [Export] private PackedScene _enemy;
    [Export] private int _fun;
    public int Fun => _fun;
    
    public EnemyBase Instantiate()
    {
        var enemy = _enemy.Instantiate<EnemyBase>();
        enemy.Fun = _fun;
        return enemy;
    }
}