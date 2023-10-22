using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.Core;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using Reactive.Bindings.Extensions;
using Timer = Godot.Timer;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

public partial class Rifle : Node2D
{
	private CooldownTimer _timer;
	private CompositeDisposable _disposable;
	private CancellationTokenSource _triggerCts;

	public void Initialize(Hero hero)
	{
		_timer = new CooldownTimer();
		_disposable = new CompositeDisposable();
		hero.Brain.LeftTrigger
			.Where(_ => !hero.IsDead)
			.Subscribe(Trigger).AddTo(_disposable);
	}

	public override void _ExitTree()
	{
		_disposable?.Dispose();
		base._ExitTree();
	}

	private void Trigger(bool isOn)
	{
		if (isOn)
		{
			_triggerCts = new CancellationTokenSource();
			TriggerOn(_triggerCts.Token).Forget();
		}
		else
		{
			TriggerOff();
		}
	}

	private async GDTask TriggerOn(CancellationToken ct)
	{
		while (ct.IsCancellationRequested == false)
		{
			if(_timer.IsCompleted) Shot();
			await _timer.CountAsync(0.75f, ct);
		}
	}

	private void TriggerOff()
	{
		_triggerCts?.Cancel();
	}

	private void Shot()
	{
		GD.Print("Shot");
	}
}
