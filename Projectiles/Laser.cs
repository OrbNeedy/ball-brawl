using Godot;
using System;

public partial class Laser : Projectile
{
    [Export]
    public Ball BallOwner;

    public override void OnReady()
    {
        Rotation = Velocity.Angle();
        Velocity = Vector2.Zero;
    }

    public override void OnProcess(double delta)
    {
        if (Scale.Y < 1)
        {
            Monitoring = false;
            Monitorable = false;
            Scale = new Vector2(1, Scale.Y + (float)(delta * 2));
        } else
        {
            Monitoring = true;
            Monitorable = true;
            Scale = Vector2.One;
        }
    }

    public override bool PreTimeLeft(double delta)
    {
        if (Scale.Y > 0.1)
        {
            Scale = new Vector2(1, Scale.Y - (float)(delta * 3));
            return false;
        }

        return base.PreTimeLeft(delta);
    }
}
