using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using Fractural.Tasks;
using Reactive.Bindings;

namespace photon.InGame.Entities.Actors.Heroes;

public class Health : IDisposable
{
	private readonly CancellationTokenSource _cts;
	private readonly ReactivePropertySlim<int> _current;
	private readonly ReactivePropertySlim<int> _max;
	private readonly Subject<Unit> _onDeath;
	private readonly ReactivePropertySlim<bool> _isInvisible;
	private readonly Subject<Unit> _onDamage;
	public IReadOnlyReactiveProperty<int> Current => _current;
	public IReadOnlyReactiveProperty<int> Max => _max;
	public IObservable<Unit> OnDeath => _onDeath;
	public IReadOnlyReactiveProperty<bool> IsInvisible => _isInvisible;
	public IObservable<Unit> OnDamage => _onDamage;
	public bool IsDead => _current.Value <= 0;
	
	public Health(int max)
	{
		_current = new ReactivePropertySlim<int>(max);
		_max = new ReactivePropertySlim<int>(max);
		_onDamage = new Subject<Unit>();
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
	
	public bool TakeDamage(int amount)
	{
		if (IsDead) return false;
		if (_isInvisible.Value) return false;
		_current.Value -= amount;
		if (IsDead)
		{
			Die();
			return true;
		}
		_onDamage.OnNext(Unit.Default);
		Invisible(2.5f, _cts.Token).Forget();
		return false;
	}
	
	public async GDTask Invisible(float duration, CancellationToken ct)
	{
		_isInvisible.Value = true;
		await GDTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
		_isInvisible.Value = false;
	}
	public void Heal(int amount)
	{
		if (IsDead) return;
		_current.Value = Math.Min(_current.Value + amount, _max.Value);
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
