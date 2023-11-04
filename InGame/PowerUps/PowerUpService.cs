using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;
using Fractural.Tasks;
using Godot;
using Godot.Extensions;
using photon.InGame.Entities.Actors.Heroes;
using Reactive.Bindings.Extensions;

namespace photon.InGame.PowerUps;

public partial class PowerUpService : Resource
{
    [Export] private WeaponPack[] _initWeaponPacks;
    private List<WeaponPack> _weaponPacks;
    private List<PowerUpBase> _powerUps;
    
    private CompositeDisposable _disposables;
    private Hero _player;
    private PowerUpUi _ui;
    private int _level = 0;
    private int _nextLevelUpChip = 5;
    private Subject<Unit> _onLevelUp;
    public IObservable<Unit> OnLevelUp => _onLevelUp;
    
    public void Initialize(Hero player, PowerUpUi ui, CancellationToken ct)
    {
        _level = 0;
        _nextLevelUpChip = 5;
        _onLevelUp = new Subject<Unit>();
        _weaponPacks = _initWeaponPacks.ToList();
        _powerUps = new List<PowerUpBase>();
        _disposables = new CompositeDisposable();
        _player = player;
        _ui = ui;
        _player.Chips.Subscribe(amount => OnEarnedChip(amount, ct)).AddTo(_disposables);
    }
    
    public void UnRegister()
    {
        _disposables?.Dispose();
        _onLevelUp?.Dispose();
    }

    private void OnEarnedChip(int totalChip, CancellationToken sessionCt)
    {
        if (totalChip < _nextLevelUpChip) return;
        CallUi(sessionCt).Forget();
    }

    private async GDTask CallUi(CancellationToken sessionCt)
    {
        _player.SubChips(_nextLevelUpChip);
        _player.Health.Heal(1);
        _player.Invisible(0.5f, sessionCt).Forget();
        _level++;
        GainNextLevelUpChip();
        var weaponTempList = new List<WeaponPack>(_weaponPacks);
        var chooses = PickPowerUpChoices(weaponTempList);
        var powerUp = await _ui.Call(chooses, sessionCt);
        powerUp.Apply(_player.WeaponHand);
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
    
    private void GainNextLevelUpChip()
    {
        const int baseValue = 5;
        const float levelUpRate = 1.32f;
        _nextLevelUpChip = Mathf.RoundToInt(baseValue * Mathf.Pow(levelUpRate, _level)) + _level * 2;
    }
    
    private IReadOnlyList<PowerUpBase> PickPowerUpChoices(List<WeaponPack> weapons)
    {
        var choices = new List<PowerUpBase>();
        weapons.Shuffle();
        var shuffledWeaponQueue = new Queue<WeaponPack>(weapons);
        _powerUps.Shuffle();
        var shuffledPowerUpQueue = new Queue<PowerUpBase>(_powerUps);
        for (var i = 0; i < 3; i++)
        {
            var powerUp = PickPowerUp(shuffledWeaponQueue, shuffledPowerUpQueue);
            choices.Add(powerUp);
        }

        return choices;
    }
    
    /// <summary>
    /// パワーアップをひとつピックする。
    /// Weaponがピックされる可能性もある。
    /// </summary>
    /// <returns></returns>
    private PowerUpBase PickPowerUp(Queue<WeaponPack> shuffledWeaponQueue, Queue<PowerUpBase> shuffledPowerUpQueue)
    {
        // プレイヤーの武器が0なら100%、1なら25%、2なら6.25%、3なら1.5625%の確率で武器がピックされる。
        // =最低1つは選出される確立：　0：100%, 1：57.8125%, 2：17.602539%, 3：4.614639%
        var weaponPickChance = Mathf.Pow(0.25f, _player.WeaponHand.WeaponCount);
        var pick = GD.Randf();
        var weaponPick = pick < weaponPickChance && shuffledWeaponQueue.Count > 0;
        WeaponPack weapon = null;
        if (weaponPick)
        {
            weapon = shuffledWeaponQueue.Dequeue();
            return weapon;
        }

        // 武器がピックされない場合、パワーアップをピックする。
        if (shuffledPowerUpQueue.Count > 0)
        {
            var powerUp = shuffledPowerUpQueue.Dequeue();
            return powerUp;
        }
        
        // 武器ピックが選ばれず、しかしパワーアップ候補が0の場合、武器をピックする。
        if (shuffledWeaponQueue.Count > 0)
        {
            weapon = shuffledWeaponQueue.Dequeue();
            return weapon;
        }
        
        throw new Exception("パワーアップ候補が0です。");
    }
}