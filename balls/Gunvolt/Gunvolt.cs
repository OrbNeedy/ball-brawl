using Godot;
using System;
using System.Collections.Generic;

public partial class Gunvolt : Ball
{
    [Signal]
    public delegate void OnFlashfieldCreateEventHandler(double time);
    [Signal]
    public delegate void OnBallTaggedEventHandler(Ball ball);

    [Export]
    public AudioStream DartShoot;
    [Export]
    public AudioStream BallTag;

    [Export]
    public PackedScene FlashfieldScene;
    [Export]
    public PackedScene DartScene;
    [Export]
    public PackedScene GVLineScene;
    [Export]
    public Sprite2D Gun;
    [Export]
    public double MaxFlashfieldCooldown = 8;
    public double FlashfieldCooldown = 8;
    [Export]
    public double MaxDartCooldown = 4;
    public double DartCooldown = 4;
    public double ShootCooldown = 0;
    public int ShootCount = 0;
    public Vector2 AimDirection = Vector2.Right;
    [Export]
    public int MaxTags = 3;
    [Export]
    public double TagDuration = 6;

    public List<Ball> CloseBalls = new();
    
    public List<GunvoltElectricity> Arcs = new();
    public double CurrentFlashfield = 0;

    public int nextTag = 0;


    public override void OnReady()
    {
        float angle = (float)Math.Tau / (float)MaxTags;
        for (int i = 0; i < MaxTags; i++)
        {
            GunvoltElectricity arc = GVLineScene.Instantiate<GunvoltElectricity>();
            arc.BallOwner = this;
            arc.Direction = Vector2.One.Rotated(angle * i);
            arc.delay = i %2 == 0;
            OnFlashfieldCreate += arc.SetTime;
            OnBallTagged += arc.EnemyTagged;
            Arcs.Add(arc);

            AddChild(arc);
        }
    }

    public override void OnProcess(double delta)
    {
        if (FlashfieldCooldown <= 0)
        {
            Flashfield flashfield = FlashfieldScene.Instantiate<Flashfield>();
            flashfield.owner = this;
            CurrentFlashfield = 2;
            flashfield.TimeLeft = CurrentFlashfield;
            flashfield.TeamName = TeamName;
            
            // EmitSignal(SignalName.SpawnProjectile, flashfield);
            AddChild(flashfield);
            EmitSignal(SignalName.OnFlashfieldCreate, CurrentFlashfield);
            FlashfieldCooldown = MaxFlashfieldCooldown;
        } else
        {
            FlashfieldCooldown -= delta;
        }
        if (CurrentFlashfield > 0)
        {
            CurrentFlashfield -= delta;
        }

        Ball closestBall = GetClosestBall();

        if (closestBall == null || !IsInstanceValid(closestBall))
        {
        } else
        {
            AimDirection = Position.DirectionTo(closestBall.Position);
        }

        // Change gun direction
        Gun.Rotation = AimDirection.Angle();
        Gun.FlipV = AimDirection.X < 0;

        ProcessDartShooting(delta);
    }

    public void ProcessDartShooting(double delta)
    {
        if (DartCooldown <= 0)
        {
            if (ShootCooldown <= 0)
            {
                Dart dart = DartScene.Instantiate<Dart>();
                dart.Position = Position + AimDirection * 38;
                dart.TeamName = TeamName;
                dart.OnBallTagged += AddTag;
                dart.Velocity = AimDirection * 400;

                EmitSignal(SignalName.SpawnProjectile, dart);
                SoundEffectManager.Instance.PlaySound(DartShoot, Position, 1f);
                ShootCount++;
                if (ShootCount >= 3)
                {
                    ShootCount = 0;
                    DartCooldown = MaxDartCooldown;
                }
                ShootCooldown = 0.15;
            } else
            {
                ShootCooldown -= delta;
            }
        } else
        {
            DartCooldown -= delta;
        }
    }

    public override bool PreBallCollision(Ball enemy)
    {
        return base.PreBallCollision(enemy);
    }

    public void AddTag(Ball ball)
    {
        Arcs[nextTag].Target = ball;
        Arcs[nextTag].TimeLeft = CurrentFlashfield;
        SoundEffectManager.Instance.PlaySound(BallTag, Position, 1f);
        EmitSignal(SignalName.OnBallTagged, ball);

        nextTag++;
        if (nextTag >= MaxTags)
        {
            nextTag = 0;
        }
    }

    public void OnDetectBall(Node2D body)
    {
        if (body.IsInGroup(TeamName)) return;

        if (body is Ball ball && !CloseBalls.Contains(ball))
        {
            CloseBalls.Add(ball);
        }
    }

    public void OnBallExitDetection(Node2D body)
    {
        if (body is Ball ball && CloseBalls.Contains(ball))
        {
            CloseBalls.Remove(ball);
        }
    }
}
