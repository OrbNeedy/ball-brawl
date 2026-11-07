using Godot;
using System;
using System.Collections.Generic;

public partial class Thanatos : Stand
{
    [Export]
    public PackedScene SlashScene;

    [Export]
    public int BraveBladeUse = 2;
    [Export]
    public double MaxBraveBladeTime = 2.25;
    public double BraveBladeTime = 1;

    public List<Ball> CloseBalls = new();

    public override void StandOutProcess(double delta)
    {
        if (BraveBladeTime <= 0)
        {
            if (BraveBladeUse > 0)
            {
                Ball closest = GetClosestBall();

                if (closest != null && IsInstanceValid(closest))
                {
                    float distance = Position.DistanceTo(closest.Position);
                    Vector2 direction = Position.DirectionTo(closest.Position);
                    if (distance <= 64)
                    {
                        BraveBlade blade = SlashScene.Instantiate<BraveBlade>();

                        blade.Position = direction * 32;
                        blade.Velocity = direction;
                        blade.OwnerSetup(StandOwner);
                        AddChild(blade);
                        BraveBladeTime = MaxBraveBladeTime;
                        BraveBladeUse--;
                    } else
                    {
                        Velocity = direction * 600;
                        return;
                    }
                }
            }
        } else
        {
            BraveBladeTime -= delta;
        }

        MoveToOwner(new Vector2(-40, -64));
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
