using System.Collections.Generic;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.PowerUps;

public abstract partial class PowerUpBase : Resource
{
    [Export] private Texture2D _describe;
    public Texture2D Describe => _describe;
    
    public abstract void Apply(WeaponHand weaponHand);
    public abstract IReadOnlyList<PowerUpBase> GetNextPowerUps();
}