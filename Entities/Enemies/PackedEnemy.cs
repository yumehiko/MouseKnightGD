using Godot;

namespace MouseKnightGD.Entities.Enemies;

public class PackedEnemy
{
    public PackedScene Pack { get; }
    public double Fun { get; }
    
    public PackedEnemy(PackedScene pack)
    {
        // MEMO: Funを取得するためだけに一度インスタンス化している。Scriptable Objectっぽいことをしたかったが……。
        Pack = pack;
        var instance = pack.Instantiate<EnemyBase>();
        Fun = instance.Fun;
        instance.QueueFree();
    }
    
    public EnemyBase Instantiate()
    {
        return Pack.Instantiate<EnemyBase>();
    }
}