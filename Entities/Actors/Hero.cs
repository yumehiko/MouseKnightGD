using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using Godot;
using MouseKnightGD.Entities.Actors.Brains;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.Entities.Actors;

/// <summary>
/// ヒーロー。このゲームにおけるプレイヤーキャラクター。
/// </summary>
public partial class Hero : Node, IEntity
{
    [Export] private RigidBody2D _body;
    [Export] private Sprite2D _visual;

    private CompositeDisposable _disposable;
    private Subject<Unit> _onRemove;
    
    public Vector2 Position => _body.Position;
    public IObservable<Unit> OnRemove => _onRemove;
    public IBrain Brain { get; private set; }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize(IBrain brain)
    {
        _disposable = new CompositeDisposable();
        _onRemove = new Subject<Unit>();
        Brain = brain;
        brain.WayPoint.Subscribe(MoveTo).AddTo(_disposable);
    }
    
    public void Remove()
    {
        // このヒーローを削除する
        _onRemove.OnCompleted();
        QueueFree();
    }
    
    private void MoveTo(Vector2 wayPoint)
    {
        _body.Position = wayPoint;
    }
}