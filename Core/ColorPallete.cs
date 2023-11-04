using Godot;

namespace photon.Core;

public static class ColorPalette
{
    public static Color PhotonWhite { get; } = new Color(0.96875f, 0.96875f, 0.96875f, 1.0f);
    public static Color GuideInactive { get; } = new Color(1, 1, 1, 0.125f);
    public static Color GuideActive { get; } = new Color(1, 1, 1, 0.375f);
    public static Color HealthGuide { get; } = new Color(1.0f, 1.0f, 1.0f, 0.4f);
    public static Color HealthGuideEmpty { get; } = new Color(1.0f, 1.0f, 1.0f, 0.1f);
    
    public static Color ChargeColor { get; } = new Color(0.6015625f, 0.8828125f, 1.0f, 0.6f);
}