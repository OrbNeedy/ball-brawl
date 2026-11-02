using Godot;
using System;

public partial class VisualClone : AnimatedSprite2D
{
    [Export]
    public Ball TargetBall;

    public override void _Process(double delta)
    {
        if (TargetBall == null || !IsInstanceValid(TargetBall))
        {
            TargetBall = null;
            QueueFree();
            return;
        }

        Animation = TargetBall.Body.Animation;
        Frame = TargetBall.Body.Frame;
        //Modulate = TargetBall.Modulate;
        //SelfModulate = TargetBall.SelfModulate;
        Scale = TargetBall.Body.Scale;
    }
}
