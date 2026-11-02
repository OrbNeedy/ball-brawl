using Godot;
using System;

public partial class TranscendantBullets : Projectile
{
    [Export]
    public int TeleportLimit = 2;
    public float ContinuousRotation = 0;
    [Export]
    public int Color = 0;

    public override void OnReady()
    {
        body.Frame = int.Clamp(Color, 0, 7);
    }

    public override void OnProcess(double delta)
    {
        Velocity = Velocity.Rotated(ContinuousRotation * (float)delta);

        if (TeleportLimit <= 0) return;

        Vector2 newPosition = Vector2.Zero;
        if (Position.X <= 0)
        {
            newPosition.X = GetViewportRect().Size.X - 1;
        }
        if (Position.Y <= 0)
        {
            newPosition.Y = GetViewportRect().Size.Y - 1;
        }
        if (Position.X >= GetViewportRect().Size.X)
        {
            newPosition.X = 1;
        }
        if (Position.Y >= GetViewportRect().Size.Y)
        {
            newPosition.Y = 1;
        }

        if (newPosition.X == 0)
        {
            newPosition.X = Position.X;
        }
        if (newPosition.Y == 0)
        {
            newPosition.Y = Position.Y;
        }

        if (newPosition != Position)
        {
            TeleportLimit--;
            Position = newPosition;
        }
    }
}
