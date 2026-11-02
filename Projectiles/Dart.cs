using Godot;
using System;

public partial class Dart : Projectile
{
    [Signal]
    public delegate void OnBallTaggedEventHandler(Ball ball);

    public override void OnProcessVisuals(double delta)
    {
        Rotation = Velocity.Angle();
    }

    public override void OnBallHit(Ball ball)
    {
        EmitSignal(SignalName.OnBallTagged, ball);
    }
}
