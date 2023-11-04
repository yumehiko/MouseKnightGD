using Godot;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

public abstract partial class AttackBase : Node2D
{
    public abstract void Initialize(IWeaponHand weaponHand);
}