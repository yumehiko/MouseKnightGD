using Godot;

namespace MouseKnightGD.Entities.Actors.Actions.Attacks;

public partial class AttackFactory : Node
{
    [Export] private Sword _sword;

    public void Initialize(Heroes.Hero player)
    {
        _sword.Initialize(player);
    }
}