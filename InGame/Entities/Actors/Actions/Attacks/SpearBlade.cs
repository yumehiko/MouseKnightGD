using System.Linq;
using Godot;
using photon.InGame.Entities.Enemies;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

/// <summary>
/// スピアの刃部分。攻撃回数分だけ生成される。
/// </summary>
public partial class SpearBlade : Area2D
{
	private SpearBladeVisual _visual;
	private bool _isInitialBlade = true;

	public void Initialize(SpearBladeVisual visual, bool isInitial)
	{
		_visual = visual;
		_isInitialBlade = isInitial;
		if (_isInitialBlade) return;
		ShiftAngle();
	}
	/// <summary>
	/// 角度を少しだけランダムにずらす（-6~+6度程度）
	/// </summary>
	public void ShiftAngle()
	{
		if (_isInitialBlade) return;
		Rotation = (float) GD.RandRange(-0.15f, 0.15f);
	}

	public bool Attack(int damage)
	{
		_visual.PierceAnimation(GlobalRotation);
		// 範囲内の敵にダメージを与える
		var bodies = GetOverlappingBodies();
		var enemies = bodies.OfType<IEnemy>();
		var blooded = false;
		foreach (var enemy in enemies)
		{
			var killed = enemy.TakeDamage(damage);
			if (killed) blooded = true;
		}
		ShiftAngle();
		return blooded;
	}
}
