using Godot;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using MouseKnightGD.Entities.Enemies;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;

namespace MouseKnightGD.Entities.Actors.Actions.Attacks;

/// <summary>
/// スラッシュ。
/// マウスクリック時、使用者の上方向へ範囲攻撃を行う。
/// </summary>
public partial class Slash : Node2D
{
    [Export] private Area2D _slashArea;
    [Export] private Sprite2D _visual;
    private CompositeDisposable _disposable;
    private bool _isCoolDown;
    private bool _isAttackTriggered;

    public void Initialize(Hero hero)
    {
        _disposable = new CompositeDisposable();
        hero.Brain.LeftTrigger
            .Where(_ => !_isCoolDown)
            .Where(isOn => isOn)
            .Subscribe(x => _ = Attack(hero.Position)).AddTo(_disposable);
    }

    public override void _ExitTree()
    {
        _disposable.Dispose();
        base._ExitTree();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (!_isAttackTriggered) return;
        GiveDamageToArea();
    }

    private async Task Attack(Vector2 point)
    {
        const float yOffset = -128;
        const float cooldown = 0.5f;
        GlobalPosition = point + new Vector2(0, yOffset);
        SlashAnimation();
        _isAttackTriggered = true;
        
        _isCoolDown = true;
        await Task.Delay(TimeSpan.FromSeconds(cooldown));
        _isCoolDown = false;
    }

    private void GiveDamageToArea()
    {
        // 範囲内の敵にダメージを与える
        var bodies = _slashArea.GetOverlappingBodies();
        var enemies = bodies.OfType<IEnemy>();
        foreach (var enemy in enemies)
        {
            enemy.TakeDamage(1);
        }
    }
    
    private void SlashAnimation()
    {
        // _visualを表示する
        _visual.Show();
        _visual.Modulate = new Color(1, 1, 1, 1);
        // _visualをTweenで徐々にフェードアウトする
        var tween = CreateTween();
        tween.TweenProperty(_visual, "modulate:a", 0, 0.25f);
        tween.Play();
    }
}