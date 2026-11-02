using Godot;
using System;

public partial class Knife : Projectile
{
    [Export]
    public int Color = 0;
    [Export]
    public bool AltSprite = false;
    [Export]
    public int WallBounce = 0;
    [Export]
    public RayCast2D ray;

    public override void OnReady()
    {
        if (AltSprite)
        {
            body.Animation = "lenen";
        }
        body.Frame = int.Clamp(Color, 0, 7);
    }

    public override void OnProcessVisuals(double delta)
    {
        float angle = Velocity.Angle();
        body.Rotation = angle + (float)(Math.PI / 2f);
    }

    public override void OnProcess(double delta)
    {
        ray.TargetPosition = Velocity;
    }

    public override void OnBodyCollision(Node2D body)
    {
        if (body.IsInGroup("Boundary") && WallBounce > 0)
        {
            Vector2 normal = ray.GetCollisionNormal();
            if (normal.X != 0)
            {
                Velocity.X *= -1;
            }
            if (normal.Y != 0)
            {
                Velocity.Y *= -1;
            }

            WallBounce--;
        }
    }
}
