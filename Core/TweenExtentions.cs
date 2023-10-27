using System.Threading;
using Fractural.Tasks;
using Godot;

namespace MouseKnightGD.Core;

public static class TweenExtensions
{
    public static GDTask PlayAsync(this Tween tween, CancellationToken ct)
    {
        var tcs = new GDTaskCompletionSource();
        ct.Register(() => tcs.TrySetCanceled(ct));
        tween.Finished += OnFinished;
        tween.Play();
        return tcs.Task;

        void OnFinished()
        {
            tween.Finished -= OnFinished;
            tcs.TrySetResult();
        }
    }
}