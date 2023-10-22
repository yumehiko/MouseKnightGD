using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

public partial class AttackFactory : Node
{
    [Export] private Sword _sword;

    public void Initialize(Hero player)
    {
        _sword.Initialize(player);
    }
}