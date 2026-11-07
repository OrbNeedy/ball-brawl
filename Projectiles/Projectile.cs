using Godot;
using System;
using System.Collections.Generic;

public enum AttackElement
{
    None, 
    Electric, 
    Fire, 
    Sound
}

public partial class Projectile : Area2D
{
    [Signal]
    public delegate void SpawnProjectileEventHandler(Projectile node);
    [Signal]
    public delegate void SpawnBallEventHandler(Ball node);
    [Signal]
    public delegate void SpawnParticlesEventHandler(SelfDeletingParticles node);
    [Export]
    public string TeamName = "";
    [Export]
    public int HitsLeft = 1;
    [Export]
    public DamageData Damage;
    [Export]
    public double MaxHitCooldown = 0.1;
    public double HitCooldown = 0;
    public List<Ball> ballsHit = new();

    [Export]
    public bool StoppableByFlashfield = false;
    public DamageModification Modifier = new();

    [Export]
    public double TimeLeft = 10;
    [Export]
    public AnimatedSprite2D body;
    [Export]
    public bool TimeResistant = false;
    public bool TimeStopped { get => GlobalTimeState.Instance.TimeStopDuration > 0; }
    public double TimeStopMovement = 1;
    public Vector2 Velocity = Vector2.Zero;

    public override void _Ready()
    {
        if (TeamName != "")
        {
            AddToGroup(TeamName);
        }

        OnReady();
    }

    public virtual void OnReady() 
    {
    }

    public void OwnerSetup(Ball ball)
    {
        TeamName = ball.TeamName;
        Modifier = ball.GetContactDamageModifier();
    }

    public override void _EnterTree()
    {
        OnTreeEntered();
    }

    public virtual void OnTreeEntered()
    {
        
    }

    public override void _Process(double delta)
    {
        TimeStopMovement = double.Clamp(TimeStopMovement, 0, 1);
        OnProcessVisuals(delta);
        if (!TimeResistant && TimeStopped)
        {
            body.SpeedScale = 0;
            Position += Velocity * (float)delta * (float)TimeStopMovement;

            TimeStopMovement -= delta;

            return;
        } else
        {
            body.SpeedScale = 1;
            TimeStopMovement = 1;
        }

        if (ShouldUpdatePosition())
        {
            Position += Velocity * (float)delta;
        }
        
        if (HitCooldown <= 0)
        {
            foreach (Ball ball in ballsHit)
            {
                DamageBall(ball);
            }
        } else
        {
            HitCooldown -= delta;
        }

        TimeLeft -= delta;
        if (TimeLeft <= 0)
        {
            if (PreTimeLeft(delta))
            {
                DeleteProjectile();
            }
            return;
        }

        OnProcess(delta);
    }

    public virtual bool ShouldUpdatePosition()
    {
        return true;
    }

    public void ForceHit(int penetrationRemoval = 1)
    {
        HitCooldown = MaxHitCooldown;
        HitsLeft -= penetrationRemoval;
        if (HitsLeft <= 0)
        {
            //GD.Print("Last hit used");
            DeleteProjectile();
        }
    }

    public virtual void OnProcessVisuals(double delta)
    {
    }

    public virtual void OnProcess(double delta)
    {
    }

    /// <summary>
    /// Runs before the projectile is deleted by timeout, but does not prevent it from disabling OnProcess
    /// </summary>
    /// <param name="delta"></param>
    /// <returns>True to allow the projectile to be deleted</returns>
    public virtual bool PreTimeLeft(double delta)
    {
        return true;
    }

    public void BodyCollision(Node2D body)
    {
        //GD.Print("Team name: " + TeamName);
        if (body.IsInGroup(TeamName)) return;

        if (body is Ball ball && !ballsHit.Contains(ball))
        {
            //GD.Print($"Projectile hit with additive: {Modifier.Additive} and multiplicative {Modifier.Multiplicative}");
            //GD.Print($"Final damage: {Modifier.ApplyToDamage(Damage)}");
            if (DamageBall(ball))
            {
                ballsHit.Add(ball);

                HitCooldown = MaxHitCooldown;
            }
        }

        OnBodyCollision(body);
    }

    public bool DamageBall(Ball ball)
    {
        if (!ball.PreProjectileCollision(this, Damage) && Damage.CanBeDodged) return false;

        if (TimeStopped && !ball.AffectedByTimeStop) return true;
        
        OnBallHit(ball);
        
        DamageModification modification = ModifyDamage(ball);
        modification.CompoundModification(Modifier);
        
        ball.ProjectileCollision(this, Damage, modification);

        ForceHit();

        return true;
    }

    public virtual DamageModification ModifyDamage(Ball target)
    {
        return new();
    }

    public virtual void OnBallHit(Ball ball)
    {
        
    }

    public void BodyExit(Node2D body)
    {
        if (body is Ball ball && ballsHit.Contains(ball))
        {
            ballsHit.Remove(ball);
        }
    }

    public virtual void OnBodyCollision(Node2D body)
    {
    }

    public void DeleteProjectile()
    {
        QueueFree();
        OnDelete();
    }

    public virtual void OnDelete()
    {
    }
}
