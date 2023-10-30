using System;
using System.Reactive;
using System.Reactive.Subjects;
using Godot;
using MouseKnightGD.InGame.Entities.Chips;
using Reactive.Bindings;
using Reactive.Bindings.TinyLinq;

namespace MouseKnightGD.InGame.Entities.Actors.Heroes;

public partial class ChipCollector : Area2D
{
    private ReactivePropertySlim<int> _chips;
    private ReactivePropertySlim<int> _score;
    private Subject<Unit> _onCollect;
    public IReadOnlyReactiveProperty<int> Chips => _chips;
    public IReadOnlyReactiveProperty<int> Score => _score;
    public IObservable<Unit> OnCollect => _onCollect;
    
    public void Initialize()
    {
        GD.Print("ChipCollector.Initialize");
        _chips = new ReactivePropertySlim<int>(0);
        _score = new ReactivePropertySlim<int>(0);
        _onCollect = new Subject<Unit>();
        BodyEntered += OnAreaEntered;
    }
    
    public void Disable()
    {
        BodyEntered -= OnAreaEntered;
    }
    public void SubChips(int amount) => _chips.Value -= amount;
    private void OnAreaEntered(Node2D body)
    {
        if (body is not Chip chip) return;
        _chips.Value += chip.Value;
        _score.Value += chip.Value;
        _onCollect.OnNext(Unit.Default);
        chip.QueueFree();
    }
}