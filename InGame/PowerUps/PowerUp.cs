using Godot;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PowerUp : Resource
{
	[Export] private Texture2D _describe;
	[Export] private bool _isWeapon;
	
	public Texture2D Describe => _describe;
	public bool IsWeapon => _isWeapon;
}
