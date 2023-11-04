using Godot;

namespace photon.InGame.Entities.Chips;

public partial class ChipFactory : Node
{
	[Export] private PackedScene _chipPack;
	[Export] private PackedScene _bigChipPack;
	private StageArea _stageArea;
	
	public void Initialize(StageArea stageArea)
	{
		_stageArea = stageArea;
	}

	public void Create(Vector2 point, int amount)
	{
		CallDeferred(nameof(DeferredCreate), point, amount);
	}
	
	public void CreateAtRandom()
	{
		var point = _stageArea.GetRandomPoint();
		Create(point, 1);
	}
	
	private void DeferredCreate(Vector2 point, int amount)
	{
		for (; amount > 0; amount--)
		{
			var isBig = amount >= 5;
			var chip = isBig ? _bigChipPack.Instantiate<Chip>() : _chipPack.Instantiate<Chip>();
			chip.Awake(point);
			AddChild(chip);
			if (isBig) amount -= 4;
		}
	}
}
