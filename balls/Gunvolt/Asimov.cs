using Godot;
using System;
using System.Collections.Generic;

public partial class Asimov : Ball
{
    [Export]
    public PackedScene FlashfieldScene;
    [Export]
    public AudioStream Prevasion1;
    [Export]
    public AudioStream Prevasion2;

    [Export]
    public AnimatedSprite2D PrevasionBody;
    [Export]
    public SubViewportContainer ViewportContainer;

    [Export]
    public uint InitialCollisionMasks = 0;
    [Export]
    public uint InitialCollisionLayers = 2;
    [Export]
    public float PrevasionChance = 0.08f;
    [Export]
    public double MaxPrevasionTimer = 1.2;
    public double PrevasionTimer = 0;
    [Export]
    public Color PrevasionColor = Colors.White;
    public double SineTimer = 0;

    [Export]
    public double MaxDashCooldown = 6;
    public double DashCooldown = 6;
    [Export]
    public double MaxStartupTimer = 1;
    public double StartupTimer = 0;
    public Vector2 TargetVelocity = Vector2.Zero;
    public Ball ClosestBall = null;
    public Vector2 LastDirection;

    public override void OnReady()
    {
        BasicCharacterballInit();
        (ViewportContainer.Material as ShaderMaterial).SetShaderParameter("Color", PrevasionColor);

        DashCooldown = MaxDashCooldown / 2.0;
    }

    public override void OnProcess(double delta)
    {
        ProcessPrevasion(delta);
        ProcessDash(delta);
    }

    public void ProcessPrevasion(double delta)
    {
        float progress = float.Clamp((float)(PrevasionTimer / (MaxPrevasionTimer / 2)), 0, 1);
        
        if (PrevasionTimer > 0)
        {
            float sine = (float)Math.Sin(SineTimer * 10);
            PrevasionTimer -= delta;
            if (PrevasionTimer >= MaxPrevasionTimer / 2)
            {
                SetCollisionLayerValue(2, false);
                if (Main.rng.Randf() <= 0.2)
                {
                    PrevasionBody.Offset = new Vector2(sine * progress * 64, 0);
                }
            } else
            {
                SetCollisionLayerValue(2, true);
                PrevasionBody.Offset = Vector2.Zero;
            }
        } else
        {
            SetCollisionLayerValue(2, true);
            PrevasionBody.Offset = Vector2.Zero;
        }

        ViewportContainer.Modulate = new Color(1, 1, 1, progress);
        PrevasionBody.Animation = Body.Animation;
        PrevasionBody.Frame = Body.Frame;
        Body.Modulate = Colors.White.Lerp(PrevasionColor, progress);
        SineTimer += delta;
    }

    public void ProcessDash(double delta)
    {
        if (DashCooldown > 0)
        {
            DashCooldown -= delta;
            TargetVelocity = Vector2.Zero;
            
            if (StartupTimer < MaxStartupTimer)
            {
                ClosestBall = GetClosestBall();
                LastDirection = Position.DirectionTo(ClosestBall.Position);
            }
        } else
        {
            if (StartupTimer >= MaxStartupTimer)
            {
                Flashfield flashfield = FlashfieldScene.Instantiate<Flashfield>();
                flashfield.Damage.BaseDamage = 2;
                flashfield.MaxHitCooldown = 0.08;
                flashfield.TimeLeft = 0.6;
                flashfield.OwnerSetup(this);
                flashfield.owner = this;
                flashfield.ProcessMode = ProcessModeEnum.Pausable;
                AddChild(flashfield);

                //ApplyForce(new Vector2(0, -1200));
                Vector2 velocity = new Vector2(850, 0).Rotated(Main.rng.RandfRange(0, (float)Math.Tau));
                if (ClosestBall != null && IsInstanceValid(ClosestBall))
                {
                    velocity = LastDirection * 850;
                }
                TargetVelocity = velocity;
                LinearVelocity = TargetVelocity;

                DashCooldown = MaxDashCooldown;
                StartupTimer = 0;
                GravityScale = 1;
            } else
            {
                StartupTimer += delta;
                TargetVelocity = Vector2.Zero;
                GravityScale = 0;
            }
        }
    }

    public override void ModifyForcesTransform(ref Transform2D transformation, PhysicsDirectBodyState2D state)
    {
        if (DashCooldown <= 0 && StartupTimer < MaxStartupTimer)
        {
            state.LinearVelocity = TargetVelocity;
        }
    }

    public override DamageModification ModifyAllDamage(DamageData data)
    {
        if (DashCooldown <= 0 && StartupTimer <= MaxStartupTimer && data.BaseDamage > 0)
        {
            return new(-1);
        }
        
        return base.ModifyAllDamage(data);
    }

    public override bool PreBallCollision(Ball enemy)
    {
        return base.PreBallCollision(enemy) && DecidePrevasion(enemy.ContactDamage);
    }

    public override bool PreProjectileCollision(Projectile projectile, DamageData data)
    {
        return base.PreProjectileCollision(projectile, data) && DecidePrevasion(data);
    }

    public override bool PreWeaponCollision(Weapon weapon)
    {
        return base.PreWeaponCollision(weapon) && DecidePrevasion(weapon.Damage);
    }

    public override bool PreDealUnspecifiedDamage(DamageData data)
    {
        return base.PreDealUnspecifiedDamage(data) && DecidePrevasion(data);
    }


    public bool DecidePrevasion(DamageData data)
    {
        if (Main.rng.Randf() <= PrevasionChance && PrevasionTimer <= 0 && 
            !AffectedByTimeStop && data.CanBeDodged)
        {
            PrevasionTimer = MaxPrevasionTimer;
            SoundEffectManager.Instance.PlaySound(Prevasion1, Position);
            SoundEffectManager.Instance.PlaySound(Prevasion2, Position);
            return false;
        }
        return true;
    }

    public void OnBallEnterDetectionRange(Node2D node)
    {
        if (node is Ball ball && !ball.IsInGroup(TeamName))
        {
            DetectedBalls.Add(ball);
        }
    }
    

    public void OnBallExitDetectionRange(Node2D node)
    {
        if (node is Ball ball && DetectedBalls.Contains(ball))
        {
            DetectedBalls.Remove(ball);
        }
    }
}
