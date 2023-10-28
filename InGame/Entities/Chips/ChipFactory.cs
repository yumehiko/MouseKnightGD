using Godot;

namespace MouseKnightGD.InGame.Entities.Chips;

public partial class ChipFactory : Node
{
	[Export] private PackedScene _chipPack;
	private StageArea _stageArea;
	
	public void Initialize(StageArea stageArea)
	{
		_stageArea = stageArea;
	}

	public void Create(Vector2 point)
	{
		CallDeferred(nameof(DeferredCreate), point);
	}
	
	public void CreateAtRandom()
	{
		var point = _stageArea.GetRandomPoint();
		Create(point);
	}
	
	private void DeferredCreate(Vector2 point)
	{
		var chip = _chipPack.Instantiate<Chip>();
		chip.Awake(point);
		AddChild(chip);
	}
}
