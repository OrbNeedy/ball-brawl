using Godot;
using System;

public partial class MarisaStar : Projectile
{
    [Export]
    public double MaxRepositionTimer = 2.5;
    public bool Falling = false;

    public override void OnProcess(double delta)
    {
        if (!Falling)
        {
            MaxRepositionTimer -= delta;
            if (MaxRepositionTimer <= 0)
            {
                Falling = true;
                Rect2 vp = GetViewportRect();
                Position = new Vector2(Main.rng.RandfRange(0, vp.Size.X), -24);
                float angle = (float)Math.PI / 6;
                Velocity = new Vector2(0, 350).Rotated(Main.rng.RandfRange(-angle, angle));
            }
        }
    }
}
