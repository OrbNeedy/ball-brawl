using Godot;
using System;

public partial class OreryBall : AnimatableBody2D
{
    [Export]
    public PackedScene ProjectileScene;
    [Export]
    public PackedScene StarScene;
    [Export]
    public DamageData BaseDamage;

    public bool TimeIsStopped { get => GlobalTimeState.Instance.TimeStopDuration > 0; }

    [Export]
    public Ball BallOwner;
    public Vector2 OldPosition = Vector2.Zero;

    [Export]
    public AnimatedSprite2D Body;
    public int BallColor = 0;
    public Vector2 OffsetToZero = new Vector2(0, 1);
    public double JustStarting = 0.4;
    [Export]
    public float MoveSpeed = 150;
    [Export]
    public float RotationSpeed = 6;
    [Export]
    public double TimeLeft = 14;
    [Export]
    public double ExitTime = 5;
    public double SineTime = 0;
    [Export]
    public int InitialLife = 15;
    [Export]
    public double MaxShootTimer = 3.5;
    public double ShootTimer = 0;
    public int ShootCount = 0;
    [Export]
    public int MaxContinuousCout = 4;
    public int ContinuousCount = 0;
    public double ContinuousCooldown = 0;

    public override void _Ready()
    {
        OldPosition = Position;
        Body.Frame = BallColor;
        OffsetToZero = OffsetToZero.Rotated(BallColor * (float)Math.PI / 2);
        ShootTimer = MaxShootTimer;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (TimeIsStopped) return;

        if (BallOwner == null || !IsInstanceValid(BallOwner))
        {
            TimeLeft = 0;
            JustStarting = 0;
        } else
        {
            if (BallOwner.life <= 0) return;
        }

        OldPosition = Position;
        float sine = (float)Math.Sin(SineTime);
        OffsetToZero = OffsetToZero.Rotated((float)delta * RotationSpeed * sine);
        SineTime += delta * 0.3;

        Position = OffsetToZero;

        if (JustStarting > 0)
        {
            JustStarting -= delta;
            OffsetToZero += OffsetToZero.Normalized() * MoveSpeed * (float)delta;
            return;
        }

        if (TimeLeft <= 0)
        {
            OffsetToZero += OffsetToZero.Normalized() * MoveSpeed * 4 * (float)delta;
            if (ExitTime <= 0)
            {
                QueueFree();
            } else
            {
                ExitTime -= delta;
            }
            return;
        } else
        {
            TimeLeft -= delta;
        }

        if (ShootTimer <= 0) 
        {
            if (ShootCount%2 == 0)
            {
                Vector2 direction = Vector2.Zero.DirectionTo(Position);
                int amount = 12;
                for (int i = 0; i < amount; i++)
                {
                    CircularProjectile proj = ProjectileScene.Instantiate<CircularProjectile>();
                    proj.OwnerSetup(BallOwner);
                    proj.TeamName = BallOwner.TeamName;
                    proj.Position = BallOwner.Position + Position;
                    proj.Damage.BaseDamage = 2;
                    proj.Velocity = direction.Rotated(i * (float)Math.Tau / (float)amount) * 380;

                    BallOwner.EmitSignal(Ball.SignalName.SpawnProjectile, proj);
                }
                ShootTimer = MaxShootTimer;
                ShootCount++;
            } else
            {
                if (ContinuousCooldown <= 0)
                {
                    MarisaStar star = StarScene.Instantiate<MarisaStar>();
                    star.OwnerSetup(BallOwner);
                    float angle = (float)Math.PI / 8f;
                    star.Velocity = new Vector2(0, -300).Rotated(Main.rng.RandfRange(-angle, angle));
                    star.Position = BallOwner.Position + Position;

                    BallOwner.EmitSignal(Ball.SignalName.SpawnProjectile, star);

                    ContinuousCooldown = 0.15;
                    ContinuousCount++;
                    if (ContinuousCount >= MaxContinuousCout)
                    {
                        ContinuousCount = 0;
                        ShootTimer = MaxShootTimer;
                        ShootCount++;
                    }
                } else
                {
                    ContinuousCooldown -= delta;
                }
            }
        } else
        {
            ShootTimer -= delta;
        }
    }

    public void OnBodyHit(Node2D body)
    {
        if (BallOwner == null || !IsInstanceValid(BallOwner)) return;

        if (body.IsInGroup(BallOwner.TeamName)) return;

        if (body is Ball ball)
        {
            InitialLife -= ball.GetContactDamage(BallOwner);
            
            if (!TimeIsStopped) ball.DealUnspecifiedDamage(BaseDamage, new());
        }

        if (body is Weapon weapon)
        {
            InitialLife -= weapon.GetWeaponDamage(BallOwner);
        }

        if (InitialLife <= 0)
        {
            QueueFree();
        }
    }

    public void OnProjectileHit(Area2D area)
    {
        if (TimeIsStopped) return;

        if (BallOwner == null || !IsInstanceValid(BallOwner)) return;

        if (area.IsInGroup(BallOwner.TeamName)) return;

        if (area is Projectile projectile)
        {
            InitialLife -= projectile.Damage.BaseDamage;
            if (!TimeIsStopped) projectile.ForceHit(2);
        }
    }
}
