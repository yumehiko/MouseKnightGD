using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame.Entities.Enemies;

public partial class DamageArea : Area2D
{
    [Export] private Sprite2D _sprite2D;
    [Export] private CollisionShape2D _damageShape2D;
    
    public async Task Alert(float alertTime, CancellationToken ct)
    {
        // 一定時間警告点滅weenを発する
        // 点滅は3回行う（アルファ0=>アルファ1=>アルファ0で1回）
        // 最後に、アルファを1にする。つまり透明度の往復は0-1-0-1-0-1-0-1-Activeとなる。
        // この間隔が全部で8回あるので、アラート時間を8で割った値が1回のアラート時間となる。
        var singleAlertTime = alertTime / 8.0f;
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_sprite2D, "modulate:a", 0.5, singleAlertTime);
        tween.TweenProperty(_sprite2D, "modulate:a", 0.0, singleAlertTime);
        tween.TweenProperty(_sprite2D, "modulate:a", 0.5, singleAlertTime);
        tween.TweenProperty(_sprite2D, "modulate:a", 0.0, singleAlertTime);
        tween.TweenProperty(_sprite2D, "modulate:a", 0.5, singleAlertTime);
        tween.TweenProperty(_sprite2D, "modulate:a", 0.0, singleAlertTime);
        tween.TweenProperty(_sprite2D, "modulate:a", 0.5, singleAlertTime);
        tween.Play();
		
        // アラート時間が終わるまで待機
        await Task.Delay(TimeSpan.FromSeconds(alertTime), ct);
        _damageShape2D.Disabled = false;
    }
	
    public async Task FadeOut(float fadeTime, CancellationToken ct)
    {
        _damageShape2D.Disabled = true;
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_sprite2D, "modulate:a", 0.0, fadeTime);
        tween.Play();
        await Task.Delay(TimeSpan.FromSeconds(fadeTime), ct);
    }
}