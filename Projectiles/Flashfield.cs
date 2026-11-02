using Godot;
using System;

public partial class Flashfield : Projectile
{
    [Export]
    public Ball owner;
    [Export]
    public AudioStream StartSound;
    [Export]
    public AudioStream ContinuousSound;
    [Export]
    public AudioStream EndSound;
    public bool Using = false;
    public double DamageCooldown = 0;

    public override void OnProcessVisuals(double delta)
    {
    }

    public override void OnProcess(double delta)
    {
    }

    public override void OnTreeEntered()
    {
        SoundEffectManager.Instance.PlaySound(StartSound, Position, 1f);
    }

    public override bool PreTimeLeft(double delta)
    {
        if (body.Animation != "end")
        {
            body.Play("end");
            Monitoring = false;
            Monitorable = false;
            Using = false;
            SoundEffectManager.Instance.PlaySound(EndSound, Position, 1f);
            SoundEffectManager.Instance.StopSound(ContinuousSound);
        }
        return false;
    }

    public void OnAnimationEnd()
    {
        if (body.Animation == "default")
        {
            Monitoring = true;
            Monitorable = true;
            Using = true;
            body.Play("continuous");
            SoundEffectManager.Instance.PlaySound(ContinuousSound, Position, 1f);
        }

        if (body.Animation == "end")
        {
            DeleteProjectile();
        }
    }

    public void OnAreaEntered(Area2D area)
    {
        if (area is Projectile projectile && !area.IsInGroup(TeamName))
        {
            if (projectile.StoppableByFlashfield && projectile.Damage.BaseDamage < 2)
            {
                projectile.DeleteProjectile();
            }
        }
    }
}
