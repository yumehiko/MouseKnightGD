using Godot;

namespace MouseKnightGD.InGame.Entities.Chips;

public partial class ChipFactory : Node
{
	[Export] private PackedScene _chipPack;

	public void Create(Vector2 point)
	{
		var chip = _chipPack.Instantiate<Chip>();
		chip.Awake(point);
		AddChild(chip);
	}
}
