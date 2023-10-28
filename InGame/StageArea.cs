using System;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;

namespace MouseKnightGD.InGame;

/// <summary>
/// このステージを表すエリア。
/// </summary>
public partial class StageArea : Area2D
{
	private readonly Random _random = new Random();
	private Hero _player;
	
	public void Initialize(Hero player)
	{
		_player = player;
	}
	
	/// <summary>
	/// ステージ内の、少し安全なランダムな座標を取得する。
	/// </summary>
	/// <returns></returns>
	public Vector2 GetRandomSafetyPoint()
	{ 
		const int areaWidth = 1920;
		const int areaHeight = 1080;
		const int offset = 80;
		const int centerX = areaWidth / 2;
		var playerInRightSide = _player.Position.X >= centerX;
		var xMin = playerInRightSide ? 0 : centerX - offset;
		var xMax = playerInRightSide ? centerX : areaWidth - offset;
		const int yMin = offset;
		const int yMax = areaHeight - offset;
		var x = _random.Next(xMin, xMax);
		var y = _random.Next(yMin, yMax);
		return new Vector2(x, y);
	}
	
	/// <summary>
	/// エリアのランダムなポイントを取得する。
	/// </summary>
	/// <returns></returns>
	public Vector2 GetRandomPoint()
	{
		const int areaWidth = 1920;
		const int areaHeight = 1080;
		const int offset = 80;
		var x = _random.Next(offset, areaWidth - offset);
		var y = _random.Next(offset, areaHeight - offset);
		return new Vector2(x, y);
	}
}
