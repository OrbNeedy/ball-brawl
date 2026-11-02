using Godot;
using System;

public partial class Ragnarok : Projectile
{
    [Export]
    public AudioStream FireSound;
    [Export]
    public PackedScene FireParticles;

    public override void OnProcess(double delta)
    {
    }

    public override void OnTreeEntered()
    {
        SoundEffectManager.Instance.PlaySound(FireSound, Position, 0.8f);
    }

    public override bool PreTimeLeft(double delta)
    {
        if (body.Animation != "end")
        {
            body.Play("end");
            Monitoring = false;
            Monitorable = false;
        }
        return false;
    }

    public void OnAnimationEnd()
    {
        if (body.Animation == "default")
        {
            Monitoring = true;
            Monitorable = true;
            body.Play("continuous");

            if (FireParticles != null)
            {
                SelfDeletingParticles particle = FireParticles.Instantiate<SelfDeletingParticles>();
                particle.FollowTarget = this;
                particle.TimeLeft = TimeLeft;
                EmitSignal(SignalName.SpawnParticles, particle);
            }
        }

        if (body.Animation == "end")
        {
            DeleteProjectile();
        }
    }
}
