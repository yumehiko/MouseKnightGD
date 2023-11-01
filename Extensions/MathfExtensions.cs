using Godot;

// ReSharper disable once CheckNamespace
namespace Godot.Extensions;

public static class MathfExtensions
{
    /// <summary>
    /// 線形成長。Ratioが10%なら、10レベルで200%、20レベルで300%になる。
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="ratio"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static float LinearGrowth(float baseValue, float ratio, int level)
    {
        return baseValue * (1.0f + ratio * level);
    }
    
    /// <summary>
    /// 指数成長。Ratioが10%なら、10レベルで259%、20レベルで672%になる。
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="ratio"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static float ExponentialGrowth(float baseValue, float ratio, int level)
    {
        return baseValue * Mathf.Pow(1 + ratio, level);
    }

    /// <summary>
    /// 逆比例成長。Ratioが10%なら、10レベルで50%、20レベルで33%になる。（クールダウンやダメージ減衰率など）
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="ratio"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static float InverseProportionalDecay(float baseValue, float ratio, int level)
    {
        return baseValue * (1.0f / (1.0f + (ratio * level)));
    }


}