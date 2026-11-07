using Godot;
using System;
using System.Collections.Generic;

public partial class Echoes : Stand
{
    [Export]
    public AudioStream HitSound;
    [Export]
    public AudioStream WriteSound;
    [Export]
    public AudioStream ThreeFreezeSound;
    [Export]
    public AudioStream ThreeFreezeEffectSound;
    [Export]
    public AudioStream[] ActChangeSound;

    [Export]
    public PackedScene SizzleScene;
    [Export]
    public PackedScene WeakenScene;
    [Export]
    public Shape2D[] ActShapes;

    [Export]
    public double MaxModeChangeTimer = 30;
    [Export]
    public double ModeChangeTimer = 10;

    [Export]
    public double MaxSizzleTimer = 5;
    public double SizzleTimer = 2;

    [Export]
    public Ball ThreeFreezeBall = null;
    public bool ThreeFrozen = false;

    [Export]
    public int Stage = 0;
    public bool Grounded = false;

    public List<Ball> CloseBalls = new();

    public override void StandOutProcess(double delta)
    {
        Ball closestBall = null;
        float approachSpeed = 350;
        float distance = float.MaxValue;
        if (CloseBalls.Count > 0)
        {
            closestBall = CloseBalls[0];
            distance = closestBall.Position.DistanceTo(StandOwner.Position);
            for (int i = 1; i < CloseBalls.Count; i++)
            {
                if (body.IsInGroup(TeamName)) continue;

                float newDistance = CloseBalls[i].Position.DistanceTo(StandOwner.Position);
                if (newDistance < distance)
                {
                    closestBall = CloseBalls[i];
                    distance = newDistance;
                }
            }
        }

        switch (Stage)
        {
            case 0:
                if (!Grounded)
                {
                    Velocity = Velocity.Lerp(new Vector2(0, 500), 0.08f);
                }
                break;
            case 1:
                approachSpeed = 450;
                break;
            case 2:
                approachSpeed = 600;
                if (SizzleTimer <= 0)
                {
                    Sizzle kanji = SizzleScene.Instantiate<Sizzle>();
                    kanji.TeamName = TeamName;
                    kanji.Position = Position;

                    EmitSignal(SignalName.SpawnProjectile, kanji);
                    SoundEffectManager.Instance.PlaySound(WriteSound, Position, 0.4f);

                    SizzleTimer = MaxSizzleTimer;
                } else
                {
                    SizzleTimer -= delta;
                }
                break;
            case 3:
                Range = 275;
                approachSpeed = 750;

                bool oldFrozen = ThreeFrozen;
                ThreeFrozen = false;
                if (ThreeFreezeBall != null && IsInstanceValid(ThreeFreezeBall))
                {
                    float dist = Position.DistanceTo(ThreeFreezeBall.Position);
                    if (dist <= Range)
                    {
                        float potency = (Range - dist) / Range;

                        float baseExtra = float.Clamp(1 - ThreeFreezeBall.DefaultGravityScale, 0, 1) + 0.8f;

                        ThreeFreezeBall.ExtraGravityScale += baseExtra + (potency * 0.6f);

                        if (!oldFrozen)
                        {
                            SoundEffectManager.Instance.PlaySound(ThreeFreezeEffectSound, Position, 0.6f);
                        }
                        
                        ThreeFrozen = true;
                    } 
                } 
                break;
        }

        if (Stage != 0)
        {
            if (closestBall == null || !IsInstanceValid(closestBall))
            {
                MoveToOwner(new Vector2(-40, -64));
            } else
            {
                if (distance <= Range)
                {
                    Velocity = Position.DirectionTo(closestBall.Position) * approachSpeed;
                } else
                {
                    MoveToOwner(new Vector2(-40, -64));
                }
            }
        }

        if (ModeChangeTimer <= 0)
        {
            IncreaseStage();
            ModeChangeTimer = MaxModeChangeTimer;
        } else
        {
            ModeChangeTimer -= delta;
        }
    }

    public void IncreaseStage()
    {
        int oldStage = Stage;
        Stage = int.Clamp(Stage + 1, 0, 3);

        if (oldStage != Stage)
        {
            SoundEffectManager.Instance.PlaySound(ActChangeSound[Stage - 1], Position);
        }

        body.Animation = "act" + Stage;
        GetNode<CollisionShape2D>("CollisionShape2D").Shape = ActShapes[Stage];
        
        OutlineRadius = body.SpriteFrames.GetFrameTexture(body.Animation, body.Frame).GetWidth() / 2f;
        QueueRedraw();
    }

    public override void OnBallHit(Ball ball)
    {
        switch (Stage)
        {
            case 1:
                EchoesSoundEffect effect = WeakenScene.Instantiate<EchoesSoundEffect>();
                effect.Category = "EchoesSound";

                ball.AddStatus(effect);
                break;
            case 3:
                if (ThreeFreezeBall != ball || ThreeFreezeBall == null || !IsInstanceValid(ThreeFreezeBall))
                {
                    SoundEffectManager.Instance.PlaySound(ThreeFreezeSound, ball.Position, 0.4f);
                }
                ThreeFreezeBall = ball;
                break;
        }

        if (Stage != 0)
        {
            SoundEffectManager.Instance.PlaySound(HitSound, Position, 0.75f);
            float strength = ModifyDamage(ball).ApplyToDamage(Damage.BaseDamage);
            ball.ApplyImpulse(StandOwner.Position.DirectionTo(ball.Position) * 150 * strength);
        }
    }

    public void OwnerDamageTaken(float newLifePercent, int damageTaken, Ball ball)
    {
        ModeChangeTimer -= damageTaken * 0.5;
    }
    
    public void OnLowerBorderCollision(Node2D body)
    {
        if (body.IsInGroup("Boundary"))
        {
            Grounded = true;
            Velocity = Vector2.Zero;
        }
    }

    public override DamageModification ModifyDamage(Ball target)
    {
        switch (Stage)
        {
            case 0:
                return new((int)(Velocity.Length() / 80f));
            case 1:
            case 2:
                return new(1);
            case 3:
                return new(2);
        }

        return base.ModifyDamage(target);
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
