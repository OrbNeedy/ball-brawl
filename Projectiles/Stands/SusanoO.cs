using Godot;
using System;

// Personas inheriting from the Stand class is funny to me
public partial class SusanoO : Stand
{
    [Export]
    public Area2D Sword;
    [Export]
    public AudioStream HitSound;
    public Vector2 Offset = Vector2.Zero;

    public override void OnStandReady()
    {
        Offset = new Vector2(0, -160);
    }

    public override void StandOutProcess(double delta)
    {
        Sword.Rotate((float)(Math.PI * delta * -4));

        Position = StandOwner.Position + Offset;

        Offset = Offset.Rotated((float)(Math.Tau * delta));
    }

    public override void OnBallHit(Ball ball)
    {
        SoundEffectManager.Instance.PlaySound(HitSound, Position);
        if (ball.AffectedByTimeStop)
        {
            ball.OldLinearVelocity += Position.DirectionTo(ball.Position) * 200;
        } else
        {
            ball.ApplyImpulse(StandOwner.Position.DirectionTo(ball.Position) * 600);
        }
    }
}
