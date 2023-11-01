using System;
using System.Reactive;

namespace photon.InGame.Entities;

public interface IRemovable
{
    void Remove();
    IObservable<Unit> OnRemove { get; }
}