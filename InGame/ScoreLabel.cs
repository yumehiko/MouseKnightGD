using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Godot;
using photon.InGame.Entities.Actors.Heroes;
using Reactive.Bindings.Extensions;

namespace photon.InGame;

public partial class ScoreLabel : Label
{
    private CompositeDisposable _disposables;
    public void Initialize(Hero hero)
    {
        _disposables = new CompositeDisposable();
        hero.Score
            .Where(_=>!hero.IsDead)
            .Skip(1)
            .Subscribe(score => Text = score.ToString()).AddTo(_disposables);
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        _disposables?.Dispose();
    }
}