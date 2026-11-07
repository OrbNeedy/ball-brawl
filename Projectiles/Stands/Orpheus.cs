using Godot;
using System;
using System.Collections.Generic;

public partial class Orpheus : Stand
{
    [Export]
    public PackedScene AgiScene;
    [Export]
    public AudioStream FireSound;

    [Export]
    public double MaxAgiTime = 2;
    public double AgiTime = 1;

    public List<Ball> CloseBalls = new();

    public override void StandOutProcess(double delta)
    {
        if (AgiTime <= 0)
        {
            Ball closest = GetClosestBall();
            Vector2 vel = new Vector2(650, 0).Rotated(Main.rng.RandfRange(0, (float)Math.Tau));
            if (closest != null)
            {
                vel = Position.DirectionTo(closest.Position) * 650;
            }

            Agi agi = AgiScene.Instantiate<Agi>();
            agi.Position = Position;
            agi.Velocity = vel;
            agi.OwnerSetup(StandOwner);
            agi.Modifier = Modifier;

            EmitSignal(SignalName.SpawnProjectile, agi);

            SoundEffectManager.Instance.PlaySound(FireSound, Position, 0.8f);

            AgiTime = MaxAgiTime;
        } else
        {
            AgiTime -= delta;
        }

        MoveToOwner(new Vector2(-40, -64), 600);
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
