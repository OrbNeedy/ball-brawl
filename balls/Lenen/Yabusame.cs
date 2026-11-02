using Godot;
using System;
using System.Collections.Generic;

public partial class Yabusame : Ball
{
    [Export]
    public AudioStream ShootSound;
    [Export]
    public PackedScene BulletScene;

    
    [Export]
    public double MaxShootTimer = 9;
    public double ShootTimer = 0;
    [Export]
    public int StackAmount = 5;
    [Export]
    public float MaxBulletSpeed = 850;
    [Export]
    public float MinBulletSpeed = 200;

    [Export]
    public int ContinuousMaxCount = 4;
    public int ContinuousCount = 0;
    [Export]
    public int ContinuousMaxDirections = 5;
    [Export]
    public double MaxContinuousCooldown = 0.1f;
    public double ContinuousCooldown = 0;
    public Vector2 ContinuousPosition = Vector2.Zero;
    public Vector2 ContinuousDirection = Vector2.Left;
    
    [Export]
    public PackedScene VisualCloneScene;
    [Export]
    public float VelDamageThreshold = 180;
    public Vector2 TeleportPosition = Vector2.Zero;
    public float OldSpeed = 0;

    public override void OnReady()
    {
        BasicCharacterballInit();
        ShootTimer = MaxShootTimer / 2.0;
    }

    public override void OnEnterTree()
    {
        Rect2 VP = GetViewportRect();
        //GD.Print("Starting VP: " + GetViewportRect());

        for (int i = 0; i < 4; i++)
        {
            VisualClone clone = VisualCloneScene.Instantiate<VisualClone>();
            clone.TargetBall = this;
            clone.Position = new Vector2(0, 1).Rotated(i * (float)Math.PI / 2f) * VP.Size;

            AddChild(clone);
        }
    }

    public override void OnProcess(double delta)
    {
        if (ShootTimer <= 0)
        {
            if (Main.rng.Randf() <= 0.5)
            {
                ContinuousPosition = Position;
                ContinuousDirection = new Vector2(0, 1).Rotated(Main.rng.RandfRange(0, (float)Math.Tau));
                ContinuousCount = ContinuousMaxCount;
            } else
            {
                AimedShoot();
            }

            ShootTimer = MaxShootTimer;
        } else
        {
            ShootTimer -= delta;
        }

        ContinuousShoot(delta);

        if (Position.X <= 0)
        {
            TeleportPosition.X = GetViewportRect().Size.X - 1;
        }
        if (Position.Y <= 0)
        {
            TeleportPosition.Y = GetViewportRect().Size.Y - 1;
        }
        if (Position.X >= GetViewportRect().Size.X)
        {
            TeleportPosition.X = 1;
        }
        if (Position.Y >= GetViewportRect().Size.Y)
        {
            TeleportPosition.Y = 1;
        }
    }

    public override void OnPhysics(double delta)
    {
    }

    public void AimedShoot() 
    {
        Ball closest = GetClosestBall();
        Vector2 baseVel = new Vector2(-1, 0).Rotated(Main.rng.RandfRange(0, (float)Math.Tau));
        if (closest != null && IsInstanceValid(closest))
        {
            baseVel = Position.DirectionTo(closest.Position);
        }

        for (int i = -1; i < 2; i++)
        {
            int amount = StackAmount - int.Abs(i * 2);
            float speedChange = (MaxBulletSpeed - MinBulletSpeed) / amount;
            if (i != 0)
            {
                speedChange *= 0.6f;
            }

            for (int k = 0; k < amount; k++)
            {
                TranscendantBullets bullet = BulletScene.Instantiate<TranscendantBullets>();
                bullet.Position = Position;
                bullet.Damage.BaseDamage = 2;
                bullet.TeamName = TeamName;
                bullet.Velocity = baseVel.Rotated(0.1f * i) * (MinBulletSpeed + (speedChange * k));
                
                EmitSignal(SignalName.SpawnProjectile, bullet);
            }
            SoundEffectManager.Instance.PlaySound(ShootSound, Position, 0.6f);
        }
    }

    public void ContinuousShoot(double delta)
    {
        if (ContinuousCount <= 0) return;

        if (ContinuousCooldown <= 0)
        {
            float rotation = (float)Math.Tau / (float)ContinuousMaxDirections;
            float contRot = 0.8f;
            if (ContinuousDirection.X < 0) contRot *= -1;
            for (int i = 0; i < ContinuousMaxDirections; i++)
            {
                TranscendantBullets bullet = BulletScene.Instantiate<TranscendantBullets>();
                bullet.Position = ContinuousPosition;
                // bullet.Damage.BaseDamage = 1;
                bullet.TeamName = TeamName;
                bullet.Velocity = ContinuousDirection.Rotated(rotation * i) * 200;
                bullet.ContinuousRotation = contRot;
                bullet.TeleportLimit = 8;
                bullet.Color = 6;
                bullet.TimeLeft = 6;

                EmitSignal(SignalName.SpawnProjectile, bullet);
            }
            SoundEffectManager.Instance.PlaySound(ShootSound, ContinuousPosition, 0.6f);
            ContinuousCount--;
            ContinuousCooldown = MaxContinuousCooldown;
        } else
        {
            ContinuousCooldown -= delta;
        }
    }

    public override DamageData GetContactDamage(Ball ball)
    {
        //GD.Print("Old speed: " + OldSpeed);
        //GD.Print("Current speed: " + LinearVelocity.Length());
        int increase = (int)(LinearVelocity.Length() / VelDamageThreshold);
        DamageData final = (DamageData)base.GetContactDamage(ball).Duplicate();
        final.BaseDamage += increase;
        //GD.Print("Increase: " + increase);
        //GD.Print("Final: " + final.BaseDamage);

        return final;
    }

    public override void ModifyForcesTransform(ref Transform2D transformation, PhysicsDirectBodyState2D state)
    {
        if (TeleportPosition.X != 0)
        {
            //GD.Print($"Teleported from {transformation.Origin} to {transformation.Origin + TeleportPosition}");
            transformation.Origin.X = TeleportPosition.X;
            TeleportPosition.X = 0;
        }
        if (TeleportPosition.Y != 0)
        {
            //GD.Print($"Teleported from {transformation.Origin} to {transformation.Origin + TeleportPosition}");
            transformation.Origin.Y = TeleportPosition.Y;
            TeleportPosition.Y = 0;
        }
    }
}
