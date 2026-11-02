using Godot;
using System;

public partial class AvoidSpriteRotation : AnimatedSprite2D
{
    public override void _Process(double delta)
    {
        GlobalRotation = 0;
    }
}
