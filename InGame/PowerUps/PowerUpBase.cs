using System.Collections.Generic;
using Godot;
using photon.InGame.Entities.Actors.Heroes;

namespace photon.InGame.PowerUps;

public abstract partial class PowerUpBase : Resource
{
    [Export] private Texture2D _describe;
    public Texture2D Describe => _describe;
    
    public abstract void Apply(WeaponHand weaponHand);
    public abstract IReadOnlyList<PowerUpBase> GetNextPowerUps();
}