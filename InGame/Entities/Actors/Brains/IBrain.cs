using Godot;
using Reactive.Bindings;

namespace photon.InGame.Entities.Actors.Brains;

public interface IBrain
{
	IReadOnlyReactiveProperty<Vector2> WayPoint { get; }
	IReadOnlyReactiveProperty<bool> LeftTrigger { get; }
}
