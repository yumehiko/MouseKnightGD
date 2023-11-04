using Godot;

namespace photon.InGame.Entities.Chips;

public partial class ChipFactory : Node
{
	[Export] private PackedScene _chipPack;
	[Export] private PackedScene _bigChipPack;
	[Export] private PackedScene _biggestChipPack;
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
		while (amount > 0)
		{
			var cost = CreateChipByAmount(amount, point);
			amount -= cost;
		}
	}
	
	private int CreateChipByAmount(int amount, Vector2 point)
	{
		var cost = 0;
		Chip chip = null;
		switch (amount)
		{
			case >= 10:
				chip = _biggestChipPack.Instantiate<Chip>();
				cost = 10;
				break;
			case >= 5:
				chip = _bigChipPack.Instantiate<Chip>();
				cost = 5;
				break;
			default:
				chip = _chipPack.Instantiate<Chip>();
				cost = 1;
				break;
		}
		
		chip.Awake(point);
		AddChild(chip);
		return cost;
	}
}
