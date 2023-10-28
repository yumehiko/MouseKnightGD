using System.Collections.Generic;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PuSlashDamage : PowerUpBase
{
    public override void Apply(Hero hero)
    {
        GD.Print("PowerUp: SlashDamage");
    }

    public override IReadOnlyList<PowerUpBase> GetNextPowerUps()
    {
        return null;
    }
}