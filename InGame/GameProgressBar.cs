using System.Threading;
using Fractural.Tasks;
using Godot;
using MouseKnightGD.Core;

namespace MouseKnightGD.InGame;

public partial class GameProgressBar : ProgressBar
{
    public async GDTask StartAsync(float duration, CancellationToken ct)
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "value", 1.0f, duration);
        await tween.PlayAsync(ct);
    }
}