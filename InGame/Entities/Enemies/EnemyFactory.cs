using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using MouseKnightGD.InGame.Entities.Chips;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame.Entities.Enemies;

public partial class EnemyFactory : Node
{
	[Export] private EnemyPack[] _enemyPacks;
	[Export] private StageArea _stageArea;
	[Export] private ChipFactory _chipFactory;
	private readonly List<IEnemy> _instances = new List<IEnemy>();
	private Hero _player;
	private SceneTree _tree;
	private CompositeDisposable _disposables;

	public override void _ExitTree()
	{
		base._ExitTree();
		_disposables?.Dispose();
	}

	public void Initialize(Hero player)
	{
		_disposables = new CompositeDisposable();
		_player = player;
	}
	
	public double Create(double bore)
	{
		var spawnPosition = _stageArea.GetRandomSpawnPoint();
		
		// 敵を生成
		var enemy = GetRandomEnemy(bore);
		_instances.Add(enemy);
		enemy.OnDeath.Subscribe(_ => { }, () => _chipFactory.Create(enemy.Position)).AddTo(_disposables);
		enemy.OnRemove.Subscribe(_ => { }, () => _instances.Remove(enemy)).AddTo(_disposables);
		enemy.Initialize(spawnPosition, _player);
		return bore - enemy.Fun;
	}

	public void RemoveAll()
	{
		_instances.ForEach(x => x.Remove());
		_instances.Clear();
	}
	
	/// <summary>
	/// ランダムなエネミーを返す。ただしfunがbore以下のエネミーのみ。
	/// </summary>
	/// <param name="bore"></param>
	/// <returns></returns>
	private IEnemy GetRandomEnemy(double bore)
	{
		var funList = _enemyPacks.Where(x => x.Fun < bore).ToList();
		if (funList.Count == 0) throw new Exception("Cannot find enemy with fun < bore.");
		var random = new Random().Next(funList.Count);
		var pack = funList[random];
		var instance = pack.Instantiate();
		AddChild(instance);
		return instance;
	}
}
