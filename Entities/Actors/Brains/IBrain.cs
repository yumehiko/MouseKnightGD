using Godot;
using Reactive.Bindings;

namespace MouseKnightGD.Entities.Actors.Brains;

public interface IBrain
{
	IReadOnlyReactiveProperty<Vector2> WayPoint { get; }
	IReadOnlyReactiveProperty<bool> LeftTrigger { get; }
}
