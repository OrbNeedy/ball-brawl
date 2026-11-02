using Godot;
using System;

public partial class GodHand : Projectile
{
    [Export]
    public PackedScene holyParticles;
    [Export]
    public AudioStream HitSound;

    public override void OnReady()
    {
        SelfDeletingParticles particles = holyParticles.Instantiate<SelfDeletingParticles>();
        particles.FollowTarget = this;
        particles.DieWithTarget = true;
        particles.OffsetToTarget = new Vector2(0, -104);
        
        EmitSignal(SignalName.SpawnParticles, particles);
    }

    public override void OnProcessVisuals(double delta)
    {
        if (Velocity.Length() > 0)
        {
            Rotation = Velocity.Angle() - (float)(Math.PI / 2f);
        }
    }

    public override void OnBallHit(Ball ball)
    {
        SoundEffectManager.Instance.PlaySound(HitSound, Position);
        if (ball.AffectedByTimeStop)
        {
            ball.OldLinearVelocity += Position.DirectionTo(ball.Position) * 200;
        } else
        {
            ball.ApplyImpulse(Position.DirectionTo(ball.Position) * 800);
        }

        TimeLeft = 0;
    }

    public override bool PreTimeLeft(double delta)
    {
        Monitoring = false;
        Monitorable = false;

        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A - (float)delta);
        Velocity *= 0.98f;

        return Modulate.A <= 0;
    }
}
