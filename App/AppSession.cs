using Godot;

namespace MouseKnightGD.App;

/// <summary>
/// このアプリの実行セッション。
/// </summary>
public partial class AppSession : Node
{
    [Export] private Curtain _curtain;
    [Export] private PackedScene _titleSessionPack;
    [Export] private PackedScene _gameSessionPack;
}