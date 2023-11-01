using System.Threading;
using Fractural.Tasks;
using Godot;
using photon.Core;

namespace photon.Tests;

public partial class TimeTest : Node2D
{
     public override void _Ready()
     {
          base._Ready();
          var cts = new CancellationTokenSource();
          Test(cts.Token).Forget();
     }

     private async GDTask Test(CancellationToken ct)
     {
          var tween = CreateTween();
          tween.TweenProperty(this, "position", new Vector2(100, 100), 1f);
          GD.Print("TimeTest.Test await");
          await tween.PlayAsync(ct);
          GD.Print("TimeTest.Test end");
     }
}