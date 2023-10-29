using Godot;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

public partial class SpearBladeVisual : Sprite2D
{
	public void PierceAnimation(float angle)
	{
		Rotation = angle;
		Modulate = Colors.White;
		// _visualをTweenで徐々にフェードアウトする
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 0, 0.2f);
		tween.Play();
	}
}
