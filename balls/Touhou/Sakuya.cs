using Godot;
using System;
using System.Collections.Generic;

public partial class Sakuya : Ball
{
    [Export]
    public Shader TimeStopShader;
    [Export]
    public PackedScene KnifeScene;
    [Export]
    public double MaxKnifeTimer = 4;
    public double KnifeTimer = 0;
    [Export]
    public int MaxAttack = 5;
    public int Attack = 0;
    [Export]
    public double MaxAttackCooldown = 0.2;
    public double AttackCooldown;
    [Export]
    public double MaxTimeStopDuration = 2;
    public double TimeStopDuration = 0;
    [Export]
    public double MaxTimeStopCooldown = 8;
    public double TimeStopCooldown = 0;
    [Export]
    public int MaxContinuousCount = 3;
    [Export]
    public int MaxCircleCount = 12;

    [Export]
    public double MaxDodgeCooldown = 10;
    public double DodgeCooldown = 0;
    public Vector2 DodgePosition = Vector2.Zero;

    [Export]
    public AudioStream TimeStopSoundEffect;
    [Export]
    public AudioStream DodgeSoundEffect;

    public Vector2 LastPosition = Vector2.Zero;

    // Theme by https://www.youtube.com/watch?v=6ITqnsLhcmE

    public override void OnReady()
    {
        BasicCharacterballInit();
        DodgeCooldown = MaxDodgeCooldown / 2f;
        TimeStopCooldown = MaxTimeStopCooldown;
    }

    public override void OnProcess(double delta)
    {
        if (DodgeCooldown > 0 && !TimeStopped)
        {
            DodgeCooldown -= delta;
        }

        if (Attack > 0)
        {
            ShootContinuous(delta);
        }

        if (TimeStopCooldown <= 0)
        {
            GlobalTimeState.Instance.SetAllShaders(null);
            GlobalTimeState.Instance.SetGlobalShader(TimeStopShader);
            GlobalTimeState.Instance.StopTime(MaxTimeStopDuration, TimeStopType.OverrideExisting);
            TimeStopDuration = MaxTimeStopDuration;
            TimeStopCooldown = MaxTimeStopCooldown;

            SoundEffectManager.Instance.PlaySound(TimeStopSoundEffect, 0.6f, true);
        } else
        {
            TimeStopCooldown -= delta;
        }

        if (TimeStopDuration > 0)
        {
            TimeResistance += delta;
            TimeStopDuration -= delta;
        }

        if (KnifeTimer < MaxKnifeTimer)
        {
            KnifeTimer += delta;
            if (TimeStopped)
            {
                KnifeTimer += delta * 2;
            }
        } else
        {
            KnifeTimer = 0;

            //GD.Print("Balls count: " + DetectedBalls.Count);
            
            if (DetectedBalls.Count <= 0)
            {
                if (Main.rng.Randf() <= 0.5f)
                {
                    float rotation = (float)Math.Tau / (float)MaxCircleCount;
                    Vector2 initialVelocity = new Vector2(420, 0).Rotated(Main.rng.RandfRange(0, (float)Math.Tau));
                    for (int i = 0; i < MaxCircleCount; i++)
                    {
                        Vector2 vel = initialVelocity.Rotated(rotation * i);
                        SpawnKnife(Position, vel, 1, 1);
                    }
                } else
                {
                    LastPosition = Position;
                    Attack = MaxAttack;
                }
            } else
            {
                Ball closest = GetClosestBall();

                if (Main.rng.Randf() <= 0.5f)
                {
                    Vector2 basePosition = closest.Position;
                    float rotation = (float)Math.PI / 4f;
                    for (int i = -2; i <= 2; i++)
                    {
                        Vector2 offset = Position.DirectionTo(closest.Position).Rotated(i * rotation);
                        Vector2 position = basePosition + (offset * 200);
                        SpawnKnife(position, position.DirectionTo(closest.Position) * 180, 0, 3);
                    }
                } else
                {
                    float rotation = (float)Math.PI / 16f;
                    Vector2 dir = Position.DirectionTo(closest.Position) * 400;
                    for (int i = -2; i <= 2; i++)
                    {
                        SpawnKnife(Position, dir.Rotated(rotation * i), 3, 4);
                    }
                }
            }
        }
    }

    public void ShootContinuous(double delta)
    {
        if (AttackCooldown > 0)
        {
            AttackCooldown -= delta;
            return;
        }

        float rotation = (float)Math.Tau / (float)MaxContinuousCount;
        float baseRotation = Attack * (float)Math.PI / 24f;
        for (int i = 0; i < MaxContinuousCount; i++)
        {
            Vector2 vel = new Vector2(280, 0).Rotated((rotation * i) + baseRotation);
            SpawnKnife(LastPosition, vel, 2, 1);
        }

        Attack--;
        if (Attack > 0)
        {
            AttackCooldown = MaxAttackCooldown;
        }
    }

    public void SpawnKnife(Vector2 position, Vector2 velocity, int bounce = 0, int damage = 2)
    {
        Knife knife = KnifeScene.Instantiate<Knife>();
        knife.Position = position;
        knife.Velocity = velocity;
        knife.WallBounce = bounce;
        knife.Damage.BaseDamage = damage;
        
        knife.Color = Main.rng.RandiRange(0, 6);
        knife.TeamName = TeamName;

        EmitSignal(SignalName.SpawnProjectile, knife);
    }

    public override void ModifyForcesTransform(ref Transform2D transformation, PhysicsDirectBodyState2D state)
    {
        if (DodgePosition != Vector2.Zero)
        {
            transformation.Origin = DodgePosition;
            DodgePosition = Vector2.Zero;
            SoundEffectManager.Instance.PlaySound(DodgeSoundEffect, Position);
        }
    }

    public override DamageData GetContactDamage(Ball ball)
    {
        if (TimeStopped)
        {
            DamageData newData = (DamageData)base.GetContactDamage(ball).Duplicate();
            newData.BaseDamage += 2;
            return newData;
        }

        return base.GetContactDamage(ball);
    }

    public override bool PreBallCollision(Ball enemy)
    {
        if (DodgeCooldown <= 0 && enemy.GetContactDamage(this).BaseDamage > 0 && !TimeStopped)
        {
            Vector2 newPosition = enemy.LinearVelocity.Normalized() * 360;
            DodgePosition = enemy.Position + newPosition;

            DodgeCooldown = MaxDodgeCooldown;
            iFrames = 0.25;
            return false;
        }

        return base.PreBallCollision(enemy);
    }

    public override bool PreWeaponCollision(Weapon weapon)
    {
        if (DodgeCooldown <= 0 && !TimeStopped)
        {
            Vector2 newPosition = weapon.LinearVelocity.Normalized() * 360;
            DodgePosition = weapon.Position + newPosition;

            DodgeCooldown = MaxDodgeCooldown;
            iFrames = 0.25;
            return false;
        }

        return base.PreWeaponCollision(weapon);
    }

    public override bool PreProjectileCollision(Projectile projectile, DamageData data)
    {
        if (DodgeCooldown <= 0 && !TimeStopped)
        {
            Vector2 newPosition = projectile.Velocity.Normalized() * 360;
            DodgePosition = projectile.Position + newPosition;

            DodgeCooldown = MaxDodgeCooldown;
            iFrames = 0.25;
            return false;
        }

        return base.PreProjectileCollision(projectile, data);
    }

    public override bool PreDealUnspecifiedDamage(DamageData data)
    {
        if (DodgeCooldown <= 0 && !TimeStopped)
        {
            Vector2 newPosition = LinearVelocity.Normalized() * 360;
            DodgePosition = Position + newPosition;

            DodgeCooldown = MaxDodgeCooldown;
            iFrames = 0.25;
            return false;
        }

        return base.PreDealUnspecifiedDamage(data);
    }

    public override void OnDamaged(int damage)
    {
    }
}
