using System;
using System.Reactive;

namespace MouseKnightGD.Entities;

public interface IDieable
{
    bool IsDead { get; }
    IObservable<Unit> OnDead { get; }
    void Die();
}