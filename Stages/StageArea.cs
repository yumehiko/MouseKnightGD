using System;
using Godot;
using MouseKnightGD.Entities.Actors;
using MouseKnightGD.Entities.Enemies;
using Hero = MouseKnightGD.Entities.Actors.Heroes.Hero;

namespace MouseKnightGD.Stages;

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
	/// ステージのエリア外のランダムな座標を取得する。
	/// </summary>
	/// <returns></returns>
	public Vector2 GetRandomSpawnPoint()
	{ 
		const int areaWidth = 1920;
		const int centerX = areaWidth / 2;
		const int areaHeight = 1080;
		const int offset = 80;
		var playerInRightSide = _player.Position.X >= centerX;
		var xMin = playerInRightSide ? 0 : centerX - offset;
		var xMax = playerInRightSide ? centerX : areaWidth - offset;
		const int yMin = offset;
		const int yMax = areaHeight - offset;
		var x = _random.Next(xMin, xMax);
		var y = _random.Next(yMin, yMax);
		return new Vector2(x, y);
	}
}
