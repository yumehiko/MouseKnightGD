using Godot;

namespace MouseKnightGD.Entities.Actors.Actions.Attacks;

public partial class AttackFactory : Node
{
    [Export] private Slash _slash;

    public void Initialize(Hero player)
    {
        _slash.Initialize(player);
    }
}