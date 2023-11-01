using Godot;
using GTweensGodot.Contexts;

namespace photon.Core;

public partial class GTweensGodotUpdater : Node
{
    public override void _Process(double delta)
    {
        GodotGTweensContextNode.Context.Tick((float)delta);
    }
}