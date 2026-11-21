using Godot;
using System;

public partial class Shine : Projectile
{
    [Export]
    public int WallBounce = 4;
    [Export]
    public RayCast2D Ray;

    public override void OnProcess(double delta)
    {
        Ray.TargetPosition = Velocity;
    }

    public override void OnBodyCollision(Node2D body)
    {
        if (body.IsInGroup("Boundary") && WallBounce > 0)
        {
            Vector2 normal = Ray.GetCollisionNormal();
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
