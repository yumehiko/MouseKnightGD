using System;
using System.Reactive;

namespace MouseKnightGD.Entities;

public interface IRemovable
{
    void Remove();
    IObservable<Unit> OnRemove { get; }
}