using System;
using System.Reactive;

namespace photon.InGame.Entities;

public interface IDieable
{
    bool IsDead { get; }
    IObservable<Unit> OnDeath { get; }
    void Die();
}