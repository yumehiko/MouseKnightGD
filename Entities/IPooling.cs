using Godot;

namespace MouseKnightGD.Entities;

public interface IPooling
{
    bool InPool { get; }
    void Awake(Vector2 initPosition);
}