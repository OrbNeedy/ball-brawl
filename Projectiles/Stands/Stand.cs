using Godot;
using System;

public partial class Stand : Projectile
{
    [Export]
    public Ball StandOwner;
    [Export]
    public bool StandOut = false;
    public bool OldStandOut = false;
    public double StandOutTimer = 0;
    public bool Attacking = false;
    [Export]
    public float Range = 350f;
    [Export]
    public Color OutlineColor = Colors.Transparent;

    public override void OnReady()
    {
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
        OnStandReady();
    }

    public virtual void OnStandReady()
    {
    }

    public void SetOwner(Ball ball)
    {
        Position = ball.Position;
        StandOwner = ball;
        TeamName = ball.TeamName;
        OutlineColor = ball.Color;
        AddToGroup(TeamName);
    }

    public override void OnProcessVisuals(double delta)
    {
        if (StandOwner != null && IsInstanceValid(StandOwner))
        {
            TimeResistant = !StandOwner.AffectedByTimeStop;
        }
    }

    public override void OnProcess(double delta)
    {
        StandOutTimer += delta;
        if (OldStandOut != StandOut)
        {
            StandOutTimer = 0;
        }

        Monitoring = StandOut;
        Monitorable = StandOut;

        if (StandOwner == null || StandOwner.life <= 0)
        {
            Monitoring = false;
            Monitorable = false;

            TimeLeft = 0;
            StandOut = false;

            Velocity = Vector2.Zero;

            return;
        }

        OutlineColor = StandOwner.Color;
        OutlineColor = new Color(OutlineColor.R, OutlineColor.G, OutlineColor.B, Modulate.A);

        if (StandOut)
        {
            float standOutProgress = float.Clamp((float)StandOutTimer * 3, 0, 1);
            
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, standOutProgress);
            StandOutProcess(delta);
        } else
        {
            float standOffProgress = float.Clamp((float)StandOutTimer, 0, 1);
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1 - standOffProgress);
            Velocity = Vector2.Zero;
            Position = Position.Lerp(StandOwner.Position, standOffProgress);

            StandOffProcess(delta);
        }
        OldStandOut = StandOut;
    }

    public override void _Draw()
    {
        float radius = body.SpriteFrames.GetFrameTexture(body.Animation, body.Frame).GetWidth() / 2f;
        DrawCircle(Vector2.Zero + body.Offset, (radius * body.Scale.X) + 4f, OutlineColor, antialiased: true);
    }

    public override bool PreTimeLeft(double delta)
    {
        StandOut = false;
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A - (float)(delta));
        return Modulate.A <= 0;
    }

    public virtual void StandOffProcess(double delta)
    {
    }

    public virtual void StandOutProcess(double delta)
    {
    }

    public void MoveToOwner(Vector2 offset, float speed = 400)
    {
        if (StandOwner.LinearVelocity.X < 0)
        {
            offset.X *= -1;
        }
        float ownerDist = Position.DistanceTo(StandOwner.Position + offset);
        if (ownerDist > speed * 0.1f)
        {
            Velocity = Position.DirectionTo(StandOwner.Position + offset) * speed;
        } else
        {
            Velocity = Vector2.Zero;
            Position = StandOwner.Position + offset;
        }
    }

    public virtual bool PreOwnerDeath()
    {
        return true;
    }
}
