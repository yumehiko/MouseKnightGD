using System;
using System.Reactive;
using Godot;
using Reactive.Bindings;

namespace MouseKnightGD.InGame.Entities.Actors.Brains;

public interface IBrain
{
	IReadOnlyReactiveProperty<Vector2> WayPoint { get; }
	IReadOnlyReactiveProperty<bool> LeftTrigger { get; }
}
