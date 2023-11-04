using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Godot;
using System;
using System.Linq;
using Fractural.Tasks;
using Godot.Extensions;
using photon.InGame.Entities.Actors.Heroes;
using photon.InGame.Entities.Enemies;
using Reactive.Bindings.Extensions;

namespace photon.InGame.Entities.Actors.Actions.Attacks;

public partial class LaserBarrel : Node2D
{
	[Export] private Area2D _damageArea;
	[Export] private LaserGuide _guide;
	[Export] private Sprite2D _beamSprite;
	[Export] private CpuParticles2D _fireParticle;
	
	public void Initialize(IWeaponHand weaponHand)
	{
		_beamSprite.Reparent(weaponHand.ProjectileRoot);
		var randomAngle = Mathf.Pi * 2 * (float) GD.RandRange(0.0, 1.0);
		Rotation = randomAngle;
	}
	
	public void SetCooldownColor(bool inCd) => _guide.SetCooldownColor(inCd);
	
	public void OnChargeTick(int chargeCount)
	{
		_guide.SetChargeGuideAmount(chargeCount);
	}
	
	public async GDTask Fire(int baseDamage, int chargeCount, CancellationToken ct)
	{
		DamageToArea(baseDamage, chargeCount);
		
		_fireParticle.Amount = 32 + chargeCount * 8;
		_fireParticle.Emitting = true;
		_guide.SetChargeGuideAmount(0);
		_beamSprite.Rotation = GlobalRotation;
		_beamSprite.GlobalPosition = GlobalPosition;
		_beamSprite.Modulate = Colors.White;
		var duration = 0.5f + Mathf.Min(chargeCount * 0.1f, 2.4f);
		var fireTween = CreateTween();
		fireTween.TweenProperty(_beamSprite, "modulate:a", 0.0f, duration)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		await fireTween.PlayAsync(ct);
	}
	
	private void DamageToArea(int baseDamage, int chargeCount)
	{
		// 範囲内の敵にダメージを与える
		var bodies = _damageArea.GetOverlappingBodies();
		var enemies = bodies.OfType<IEnemy>();
		var damage = baseDamage + chargeCount * 2;
		foreach (var enemy in enemies)
		{
			enemy.TakeDamage(damage);
		}
	}
}
