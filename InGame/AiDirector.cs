using System.Threading;
using Fractural.Tasks;
using MouseKnightGD.InGame.Entities.Chips;
using MouseKnightGD.InGame.Entities.Enemies;

namespace MouseKnightGD.InGame;

/// <summary>
/// Bore値を状況に応じて増減させ、難易度を調節する。
/// </summary>
public class AiDirector
{
    private double _maxBore;
    private readonly EnemyFactory _enemyFactory;
    public AiDirector(EnemyFactory enemyFactory, double maxBore)
    {
        _enemyFactory = enemyFactory;
        _maxBore = maxBore;
    }

    public async GDTask OnBoreTick(CancellationToken ct)
    {
        // 現在のFun値。高ければ高いほど敵がいる（退屈ではない）
        var currentTotalFun = _enemyFactory.GetTotalFun();
        
        // 現在のbore値を計算
        var currentBore = _maxBore - currentTotalFun;
        var boreRatio = currentBore / _maxBore;
        
        // 十分に退屈なら、敵を生成する。1フレームに1体ずつ。
        while (currentBore > _enemyFactory.LowestFun)
        {
            currentBore = _enemyFactory.Create(currentBore);
            await GDTask.NextFrame(cancellationToken: ct);
        }
        
        // このTickの退屈比率分、maxBoreを増加させる。
        const double increaseRatio = 0.25;
        _maxBore += _maxBore * boreRatio * increaseRatio;
    }
}