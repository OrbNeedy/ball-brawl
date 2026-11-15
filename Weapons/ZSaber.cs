using Godot;
using System;

public partial class ZSaber : Weapon
{
    [Export]
    public AudioStream CutSound;
    [Export]
    public AudioStream HeavyCutSound;

    [Export]
    public PackedScene DeflectParticles;
    [Export]
    public PackedScene ChangeToDeflectParticles;
    [Export]
    public PackedScene ChangeToStandardParticles;

    [Export]
    public bool DeflectionMode = false;

    [Export]
    public double MaxModeChangeTimer = 9;
    public double ModeChangeTimer = 4;

    public override void OnProcess(double delta)
    {
        if (ModeChangeTimer <= 0)
        {
            DeflectionMode = !DeflectionMode;
            SyncVisuals();

            if (DeflectionMode)
            {
                ModeChangeTimer = MaxModeChangeTimer / 1.4f;
            } else
            {
                ModeChangeTimer = MaxModeChangeTimer;
            }
        } else
        {
            ModeChangeTimer -= delta;
        }
    }

    public void SyncVisuals()
    {
        if (DeflectionMode)
        {
            Body.Animation = "deletion";
        } else
        {
            Body.Animation = "default";
        }
    }

    public void OnProjectileHit(Area2D area)
    {
        if (area.IsInGroup(TeamName)) return;

        if (area is Projectile projectile && DeflectionMode)
        {
            projectile.OnReflect(Holder);
        }
    }

    public override DamageModification GetWeaponDamageModifications(Ball ball)
    {
        if (DeflectionMode)
        {
            //return new(2);
        }

        return base.GetWeaponDamageModifications(ball);
    }

    public override void OnBallHit(Ball ball)
    {
        if (!DeflectionMode)
        {
            SoundEffectManager.Instance.PlaySound(CutSound, ball.Position);
        } else
        {
            SoundEffectManager.Instance.PlaySound(HeavyCutSound, ball.Position);
        }
    }
}
