using Godot;
using Reactive.Bindings;

namespace MouseKnightGD.InGame.Entities.Actors.Brains;

/// <summary>
/// プレイヤーの入力を管理するクラス
/// </summary>
public partial class PlayerBrain : Node, IBrain
{
	private readonly ReactiveProperty<Vector2> _wayPoint = new ReactiveProperty<Vector2>();
	private readonly ReactiveProperty<bool> _leftTrigger = new ReactiveProperty<bool>();
	
	public IReadOnlyReactiveProperty<Vector2> WayPoint => _wayPoint;
	public IReadOnlyReactiveProperty<bool> LeftTrigger => _leftTrigger;

	public override void _ExitTree()
	{
		_wayPoint.Dispose();
		_leftTrigger.Dispose();
		base._ExitTree();
	}

	public override void _Input(InputEvent @event)
	{
		switch (@event)
		{
			case InputEventMouseButton eventMouseButton:
				_leftTrigger.Value = eventMouseButton.Pressed;
				break;
			case InputEventMouseMotion eventMouseMotion:
				_wayPoint.Value = eventMouseMotion.Position;
				break;
			default: break;
		}
	}
}
