using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

public partial class AttackFactory : Node2D
{
    [Export] private Sword _sword;
    [Export] private Rifle _rifle;

    public void Initialize(Hero player)
    {
        GD.Print("AttackFactory.Initialize");
        _sword.Initialize(player);
        _rifle.Initialize(player);
    }
}