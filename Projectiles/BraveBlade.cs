using Godot;
using System;

public partial class BraveBlade : Projectile
{
    [Export]
    public AudioStream SlashSound;

    public override void OnProcessVisuals(double delta)
    {
        if (Velocity.Length() > 0)
        {
            Rotation = Velocity.Angle();
        }

        body.FlipH = Velocity.X >= 0;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public void OnAnimationEnd()
    {
        if (body.Animation == "default")
        {
            Monitoring = false;
            Monitorable = false;
            body.Play("end");
        }

        if (body.Animation == "end")
        {
            DeleteProjectile();
        }
    }

    public override void OnBallHit(Ball ball)
    {
        SoundEffectManager.Instance.PlaySound(SlashSound, Position);
    }
}
