using Godot;
using System;
using System.Collections.Generic;

public partial class Messiah : Stand
{
    [Export]
    public AnimatedSprite2D Shine;
    public double VisualTimer = 0;

    [Export]
    public PackedScene MegidolaonScene;
    [Export]
    public PackedScene GodHandScene;
    [Export]
    public PackedScene HealParticles;
    [Export]
    public PackedScene SPParticles;
    [Export]
    public AudioStream SalvationSound;
    [Export]
    public AudioStream EnduringSound;

    [Export]
    public double MaxSelfHealingTimer = 2;
    public double SelfHealingTimer = 0;

    [Export]
    public double MaxMegidolaonTimer = 7.5;
    [Export]
    public double MaxGodHandTimer = 4;
    public double AttackTimer = 4;
    public bool MegidolaonNext = false;

    [Export]
    public int ResurrectCount = 1;
    [Export]
    public int SalvationCount = 1;

    public List<Ball> CloseBalls = new();

    public override void StandOutProcess(double delta)
    {
        ProcessVisuals(delta);
        SelfHeal(delta * 0.5);

        if (AttackTimer <= 0)
        {
            if (((float)StandOwner.life / (float)StandOwner.MaxLife) <= 0.1f && SalvationCount > 0)
            {
                StandOwner.HealBall(StandOwner.MaxLife);
            
                SoundEffectManager.Instance.PlaySound(SalvationSound, StandOwner.Position);

                SelfDeletingParticles heal = HealParticles.Instantiate<SelfDeletingParticles>();
                heal.FollowTarget = StandOwner;
                EmitSignal(SignalName.SpawnParticles, heal);
                
                SelfDeletingParticles sp = SPParticles.Instantiate<SelfDeletingParticles>();
                sp.FollowTarget = StandOwner;
                EmitSignal(SignalName.SpawnParticles, sp);

                SalvationCount--;
                AttackTimer = 30;
                return;
            }

            if (MegidolaonNext)
            {
                ShootMegidolaon();
            } else
            {
                ShootGodHand();
            }

            MegidolaonNext = Main.rng.Randf() <= 0.35;
            if (MegidolaonNext)
            {
                AttackTimer = MaxMegidolaonTimer;
            } else 
            {
                AttackTimer = MaxGodHandTimer;
            }
        } else
        {
            AttackTimer -= delta;
        }

        MoveToOwner(new Vector2(-40, -64));
    }

    public void ShootMegidolaon()
    {
        Vector2 size = GetViewportRect().Size;
        Megidolaon megidolaon = MegidolaonScene.Instantiate<Megidolaon>();
        megidolaon.OwnerSetup(StandOwner);
        megidolaon.Position = size / 2f;

        EmitSignal(SignalName.SpawnProjectile, megidolaon);
    }

    public void ShootGodHand()
    {
        Vector2 size = GetViewportRect().Size;
        Vector2 position = new Vector2(Main.rng.RandfRange(0, size.X), -64);

        Ball closest = GetClosestBall();
        Vector2 velocity = position.DirectionTo(new Vector2(Main.rng.RandfRange(0, size.X), size.Y)) * 500;
        if (closest != null && IsInstanceValid(closest))
        {
            velocity = position.DirectionTo(closest.Position) * 500;
        }

        GodHand godHand = GodHandScene.Instantiate<GodHand>();
        godHand.OwnerSetup(StandOwner);
        godHand.Position = position;
        godHand.Velocity = velocity;

        EmitSignal(SignalName.SpawnProjectile, godHand);
    }

    public override void StandOffProcess(double delta)
    {
        ProcessVisuals(delta);
        SelfHeal(delta);

        if (AttackTimer > 0)
        {
            AttackTimer -= delta;
        }
    }

    public void SelfHeal(double delta)
    {
        if (SelfHealingTimer <= 0)
        {
            StandOwner.HealBall(1);
            SelfHealingTimer = MaxSelfHealingTimer;
        } else
        {
            SelfHealingTimer -= delta;
        }
    }

    public void ProcessVisuals(double delta)
    {
        float sineX = (float)Math.Sin(VisualTimer * 0.8) + 2.8f;
        float sineY = (float)Math.Sin(0.36 + (VisualTimer * 0.65)) + 2.8f;
        float pulsate = (float)(Math.Cos(VisualTimer) + 1) * 0.5f;

        Shine.Scale = new Vector2(sineX * 0.3f, sineY * 0.3f);
        Shine.Modulate = new Color(1f, 1f, 1f, 0.8f + (pulsate * 0.2f));

        VisualTimer += delta;
    }

    public void DetectBall(Node2D body)
    {
        if (body is Ball ball)
        {
            if (CloseBalls.Contains(ball) || ball.IsInGroup(TeamName)) return;

            CloseBalls.Add(ball);
        }
    }

    public void DetectExit(Node2D body)
    {
        if (body is Ball ball)
        {
            if (!CloseBalls.Contains(ball)) return;

            CloseBalls.Remove(ball);
        }
    }

    public override bool PreOwnerDeath()
    {
        //GD.Print("Resurrect count: " + ResurrectCount);
        if (ResurrectCount > 0)
        {
            ResurrectCount--;
            StandOwner.HealBall(StandOwner.MaxLife);
            
            SoundEffectManager.Instance.PlaySound(EnduringSound, StandOwner.Position);

            SelfDeletingParticles heal = HealParticles.Instantiate<SelfDeletingParticles>();
            heal.FollowTarget = StandOwner;
            EmitSignal(SignalName.SpawnParticles, heal);

            return false;
        }
        //GD.Print("No resurrection");

        return base.PreOwnerDeath();
    }

    public Ball GetClosestBall()
    {
        Ball ClosestBall = null;
        if (CloseBalls.Count > 0)
        {
            ClosestBall = CloseBalls[0];
            float distance = ClosestBall.Position.DistanceTo(Position);

            for (int i = 1; i < CloseBalls.Count; i++)
            {
                float currentDistance = CloseBalls[i].Position.DistanceTo(Position);

                if (currentDistance < distance)
                {
                    ClosestBall = CloseBalls[i];
                    distance = currentDistance;
                }
            }
        }

        return ClosestBall;
    }
}
