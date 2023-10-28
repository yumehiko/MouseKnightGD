using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

public abstract partial class AttackBase : Node2D
{
    public abstract void Initialize(Hero hero);
}