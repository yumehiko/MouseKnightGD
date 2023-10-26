using Godot;
using GTweensGodot.Contexts;

namespace MouseKnightGD.Core;

public partial class GTweensGodotUpdater : Node
{
    public override void _Process(double delta)
    {
        GodotGTweensContextNode.Context.Tick((float)delta);
    }
}