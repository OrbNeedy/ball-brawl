using Godot;
using System;
using System.Linq;

public partial class GunvoltElectricity : Line2D
{
    [Export]
    public CompressedTexture2D[] textures;
    [Export]
    public Ball BallOwner;
    [Export]
    public Ball Target;
    public bool Inverted = false;
    [Export]
    public Vector2 Direction = Vector2.Zero;
    public bool delay = true;
    public double TimeLeft = 0;
    public double TagTime = 0;

    [Export]
    public double MaxDamageTimer = 0.4;
    public double DamageTimer = 0;

    public override void _Process(double delta)
    {
        ProcessVisuals(delta);
        ProcessOffense(delta);
    }

    public void ProcessVisuals(double delta)
    {
        if (BallOwner == null || !IsInstanceValid(BallOwner))
        {
            Points = [ Vector2.Zero ];
            return;
        }

        if (Target == null || !IsInstanceValid(Target))
        {
            Points = [ Vector2.Zero ];
            return;
        }
        
        if (BallOwner.AffectedByTimeStop) return;
        
        Points = [ Vector2.Zero ];

        Target.Reticle.TargetAnimation = "gunvolt";
        Target.Reticle.TargetFrame += 1;

        if (Target.life <= 0)
        {
            return;
        }
        
        delay = !delay;

        if (delay)
        {
            Texture = textures[Main.rng.RandiRange(0, textures.Length - 1)];
            return;
        }

        if (TimeLeft <= 0)
        {
            //GD.Print("Time up");
            return;
        }

        Vector2 FlashfieldBorder = (Direction * 148);

        Direction = Direction.Rotated(1 * (float)delta);

        Points = Points.Append(FlashfieldBorder).ToArray();

        Vector2 targetPos = Target.Position - BallOwner.Position;
        //GD.Print("Target position: " + Target.Position);
        
        Vector2 control1 = FlashfieldBorder + (Direction * 8).Lerp(targetPos, 0.1f);
        control1 += control1.Lerp(targetPos, 0.1f);
        Vector2 control2 = targetPos + (targetPos.DirectionTo(BallOwner.Position) * 64);
        
        int points = (int)(FlashfieldBorder.DistanceTo(targetPos) / 100f);
        float progress = 1f / (float)points;
        for (int i = 0; i < points; i++)
        {
            //GD.Print($"Setting point {i}");
            Vector2 offset = new Vector2(Main.rng.RandfRange(-64, 64), Main.rng.RandfRange(-64, 64));
            Vector2 point = FlashfieldBorder.BezierInterpolate(control1, control2 + offset, targetPos, i * progress);
            Points = Points.Append(point).ToArray();
        }

        Points = Points.Append(targetPos).ToArray();
    }

    public void ProcessOffense(double delta)
    {
        if (BallOwner == null || !IsInstanceValid(BallOwner))
        {
            return;
        }

        if (BallOwner.life <= 0) return;

        if (BallOwner.AffectedByTimeStop) return;

        if (DamageTimer > 0 )
        {
            DamageTimer -= delta;
        }

        //GD.Print($"Time left: {TimeLeft}");
        if (TimeLeft <= 0)
        {
            //GD.Print("Time up");
            return;
        } else
        {
            TimeLeft -= delta;
        }

        if (TagTime > 0)
        {
            TagTime -= delta;
        } else
        {
            Target = null;
        }

        if (Target == null || !IsInstanceValid(Target))
        {
            //GD.Print("Target invalid");
            TagTime = 0;
            return;
        }

        if (Target.life <= 0)
        {
            Target = null;
            return;
        }

        if (DamageTimer <= 0)
        {
            Target.DamageBall(new DamageData() { BaseDamage = 1, Almighty = true, CanBeDodged = false });
            DamageTimer = MaxDamageTimer;
        }
    }

    public void EnemyTagged(Ball ball)
    {
        if (Target == ball)
        {
            TagTime = 5;
        }
    }

    public void SetTime(double time)
    {
        //GD.Print("Setting time");
        if (Target == null || !IsInstanceValid(Target))
        {
            return;
        }
        //GD.Print("Success");

        TimeLeft = time;
    }
}
