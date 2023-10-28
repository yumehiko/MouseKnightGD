using System;
using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using MouseKnightGD.InGame.Entities.Actors.Heroes;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using Godot;
using Reactive.Bindings.Extensions;

namespace MouseKnightGD.InGame.PowerUps;

public partial class PowerUpService : Resource
{
    [Export] private WeaponPack[] _initWeaponPacks;
    private List<WeaponPack> _weaponPacks;
    private List<PowerUpBase> _powerUps;
    
    private CompositeDisposable _disposables;
    private Hero _player;
    private PowerUpUi _ui;
    private int _nextLevelUpChip = 5;
    private Subject<Unit> _onLevelUp;
    public IObservable<Unit> OnLevelUp => _onLevelUp;
    
    public void Initialize(Hero player, PowerUpUi ui, CancellationToken ct)
    {
        _onLevelUp = new Subject<Unit>();
        _weaponPacks = _initWeaponPacks.ToList();
        _powerUps = new List<PowerUpBase>();
        _disposables = new CompositeDisposable();
        _player = player;
        _ui = ui;
        _player.Chips.Subscribe(amount => OnEarnedChip(amount, ct)).AddTo(_disposables);
    }

    protected override void Dispose(bool disposing)
    {
        _disposables?.Dispose();
        _onLevelUp?.Dispose();
        base.Dispose(disposing);
    }

    private void OnEarnedChip(int chip, CancellationToken sessionCt)
    {
        if (chip < _nextLevelUpChip) return;
        CallUi(sessionCt).Forget();
    }

    private async GDTask CallUi(CancellationToken sessionCt)
    {
        _player.SubChips(_nextLevelUpChip);
        // _nextLevelUpChip += 5; TODO: 次のレベルアップに必要な経験治療を増加させる。レベルアップ曲線を考える
        var weaponTempList = new List<WeaponPack>(_weaponPacks);
        var chooses = PickPowerUpChoices(weaponTempList);
        var powerUp = await _ui.Call(chooses, sessionCt);
        powerUp.Apply(_player);
        // このパワーアップから、追加のパワーアップ候補を取得する。
        var nextPowerUps = powerUp.GetNextPowerUps();
        if (nextPowerUps == null || nextPowerUps.Count == 0) return;
        // パワーアップ候補に追加する
        _powerUps.AddRange(nextPowerUps);
        // このパワーアップが武器だった場合、武器候補から除外する（同じ武器はピックできない）
        if (_weaponPacks.Contains(powerUp))
        {
            _weaponPacks.Remove((WeaponPack) powerUp);
        }
        _onLevelUp.OnNext(Unit.Default);
    }
    
    private IReadOnlyList<PowerUpBase> PickPowerUpChoices(List<WeaponPack> weaponQueue)
    {
        var result = new List<PowerUpBase>();
        for (var i = 0; i < 3; i++)
        {
            var powerUp = PickPowerUp(weaponQueue);
            result.Add(powerUp);
        }

        return result;
    }
    
    /// <summary>
    /// パワーアップをひとつピックする。
    /// Weaponがピックされる可能性もある。
    /// </summary>
    /// <returns></returns>
    private PowerUpBase PickPowerUp(List<WeaponPack> weaponQueue)
    {
        // プレイヤーの武器が0なら100%、1なら25%、2なら6.25%、3なら1.5625%の確率で武器がピックされる。
        var weaponPickChance = Mathf.Pow(0.25f, _player.WeaponCount);
        var pick = GD.Randf();
        
        if (pick < weaponPickChance)
        {
            var wpId = GD.RandRange(0, _weaponPacks.Count - 1);
            var weapon = _weaponPacks[wpId];
            weaponQueue.Remove(weapon);
            return weapon;
        }
        
        var index = GD.RandRange(0, _powerUps.Count - 1);
        var powerUp = _powerUps[index];
        return powerUp;
    }
}