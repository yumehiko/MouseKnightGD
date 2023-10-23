using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using Fractural.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.TinyLinq;

namespace MouseKnightGD.InGame.Entities.Actors.Heroes;

public class Health : IDisposable
{
	private CancellationTokenSource _cts;
	private readonly ReactivePropertySlim<int> _current;
	private readonly ReactivePropertySlim<int> _max;
	private readonly ReactiveProperty<float> _normalized;
	private readonly Subject<Unit> _onDeath;
	private readonly ReactivePropertySlim<bool> _isInvisible;
	public IReadOnlyReactiveProperty<int> Current => _current;
	public IReadOnlyReactiveProperty<int> Max => _max;
	public IReadOnlyReactiveProperty<float> Normalized => _normalized;
	public IObservable<Unit> OnDeath => _onDeath;
	public IReadOnlyReactiveProperty<bool> IsInvisible => _isInvisible;
	public bool IsDead => _current.Value <= 0;
	
	public Health(int max)
	{
		_current = new ReactivePropertySlim<int>(max);
		_max = new ReactivePropertySlim<int>(max);
		_normalized = _current.Select(x => (float)x / _max.Value).ToReactiveProperty();
		_onDeath = new Subject<Unit>();
		_isInvisible = new ReactivePropertySlim<bool>();
		_cts = new CancellationTokenSource();
	}

	public void Dispose()
	{
		_current?.Dispose();
		_max?.Dispose();
		_cts?.Cancel();
	}
	
	public void TakeDamage(int amount)
	{
		if (IsDead) return;
		if (_isInvisible.Value) return;
		_current.Value -= amount;
		if (IsDead)
		{
			_onDeath.OnNext(Unit.Default);
			_onDeath.OnCompleted();
			return;
		}
		
		Invisible(2.5f, _cts.Token).Forget();
	}
	
	private async GDTask Invisible(float duration, CancellationToken ct)
	{
		_isInvisible.Value = true;
		await GDTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
		_isInvisible.Value = false;
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
