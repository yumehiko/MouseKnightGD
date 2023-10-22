using System;
using System.Reactive;
using System.Reactive.Subjects;
using Reactive.Bindings;
using Reactive.Bindings.TinyLinq;

namespace MouseKnightGD.InGame.Entities.Actors.Heroes;

public class Health : IDisposable
{
	private readonly ReactivePropertySlim<int> _current;
	private readonly ReactivePropertySlim<int> _max;
	private readonly ReactiveProperty<float> _normalized;
	private readonly Subject<Unit> _onDeath = new Subject<Unit>();
	public IReadOnlyReactiveProperty<int> Current => _current;
	public IReadOnlyReactiveProperty<int> Max => _max;
	public IReadOnlyReactiveProperty<float> Normalized => _normalized;
	public IObservable<Unit> OnDeath => _onDeath;
	public bool IsDead => _current.Value <= 0;
	
	public Health(int max)
	{
		_current = new ReactivePropertySlim<int>(max);
		_max = new ReactivePropertySlim<int>(max);
		_normalized = _current.Select(x => (float)x / _max.Value).ToReactiveProperty();
	}

	public void Dispose()
	{
		_current?.Dispose();
		_max?.Dispose();
	}
	
	public void TakeDamage(int amount)
	{
		if (IsDead) return;
		_current.Value -= amount;
		if (IsDead)
		{
			_onDeath.OnNext(Unit.Default);
			_onDeath.OnCompleted();
		}
	}
	
	public void FullHeal()
	{
		if (IsDead) return;
		_current.Value = _max.Value;
	}
	
	public void Resurrection()
	{
		_current.Value = _max.Value;
	}

	public void Die()
	{
		_current.Value = 0;
		_onDeath.OnNext(Unit.Default);
		_onDeath.OnCompleted();
	}
}
