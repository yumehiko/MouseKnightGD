using Godot;

namespace MouseKnightGD.InGame.Entities.Actors.Actions.Attacks;

public partial class SpearBladeVisual : Sprite2D
{
	private Tween _tween;
	public void PierceAnimation(float angle)
	{
		Rotation = angle;
		Modulate = Colors.White;
		// _visualをTweenで徐々にフェードアウトする
		_tween?.Kill();
		_tween = CreateTween();
		_tween.TweenProperty(this, "modulate:a", 0, 0.2f);
		_tween.Play();
	}
}
