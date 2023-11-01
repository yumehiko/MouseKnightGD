using Godot;

namespace photon.InGame.Entities.Enemies;

public partial class EnemyVisual : Sprite2D
{
    public void Damage(float normalizedHp)
    {
        // alphaは正規化されたHPを、0.25-1.0の範囲に変換する
        var alpha = normalizedHp * 0.75f + 0.25f;
        SelfModulate = new Color(Modulate.R, Modulate.B, Modulate.G, alpha);
    }
}