using System;
using System.Reactive;

namespace MouseKnightGD.InGame.Entities;

public interface IRemovable
{
    void Remove();
    IObservable<Unit> OnRemove { get; }
}