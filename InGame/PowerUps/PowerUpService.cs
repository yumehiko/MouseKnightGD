using System;
using Fractural.Tasks;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using Godot;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame.PowerUps;

public class PowerUpService : IDisposable
{
    private readonly CompositeDisposable _disposables;
    private readonly Hero _player;
    private readonly PowerUpUi _ui;
    private CancellationToken _sessionCt;
    
    public PowerUpService(Hero player, PowerUpUi ui, CancellationToken ct)
    {
        _disposables = new CompositeDisposable();
        _player = player;
        _ui = ui;
        _sessionCt = ct;
        _player.Chips.Subscribe(OnEarnedChip).AddTo(_disposables);
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }

    private void OnEarnedChip(int chip)
    {
        if (chip < 5) return;
        _player.SubChips(5);
        _ui.Call(_sessionCt).Forget();
    }
}