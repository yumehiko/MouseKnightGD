using Godot;

namespace MouseKnightGD.InGame.Entities.Enemies;

public partial class EnemyPack : Resource
{
    [Export] private PackedScene _enemy;
    [Export] private int _fun;
    
    public PackedScene Enemy => _enemy;
    public int Fun => _fun;
}