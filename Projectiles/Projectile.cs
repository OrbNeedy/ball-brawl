using Godot;
using System;
using System.Collections.Generic;

public enum AttackElement
{
    None, 
    Electric, 
    Fire
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
    [Export]
    public AttackElement Element = AttackElement.None;

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
                //GD.Print("Process damaged ball " + ball.BallName);
                if (!ball.PreProjectileCollision(this, Damage) && Damage.CanBeDodged) continue;
                
                OnBallHit(ball);
                ball.ProjectileCollision(this, Damage);

                HitCooldown = MaxHitCooldown;
                HitsLeft--;
                if (HitsLeft <= 0)
                {
                    //GD.Print("Last hit used");
                    DeleteProjectile();
                }
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
            if (TimeStopped && !TimeResistant && !ball.AffectedByTimeStop) return;
            //GD.Print("Collision damaged ball " + ball.BallName);
            //GD.Print("Ball hit");
            if (!ball.PreProjectileCollision(this, Damage) && Damage.CanBeDodged)
            {
            } else
            {
                OnBallHit(ball);
                if (ball.iFrames <= 0)
                {
                    ball.ProjectileCollision(this, ModifyDamage(ball));
                }

                HitsLeft--;
                if (HitsLeft <= 0)
                {
                    //GD.Print("Last hit used");
                    DeleteProjectile();
                }
            }

            ballsHit.Add(ball);

            HitCooldown = MaxHitCooldown;
        }

        OnBodyCollision(body);
    }

    public virtual DamageData ModifyDamage(Ball target)
    {
        return Damage;
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
