using System;
using System.Reactive;
using Godot;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MouseKnightGD.Entities;

public interface IEntity
{
    Vector2 Position { get; }
    void Remove();
    IObservable<Unit> OnRemove { get; }
}