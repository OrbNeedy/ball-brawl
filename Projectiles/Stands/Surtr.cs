using Godot;
using System;
using System.Collections.Generic;

public partial class Surtr : Stand
{
    [Export]
    public PackedScene RagnarokScene;
    [Export]
    public double MaxRagnarokCooldown = 6;
    public double RagnarokCooldown = 1;
    public List<Ball> CloseBalls = new();

    public override void OnStandReady()
    {
        RagnarokCooldown = 1;
    }

    public override void StandOutProcess(double delta)
    {
        if (RagnarokCooldown <= 0)
        {
            Vector2 ragnarokPos = StandOwner.Position;
            if (CloseBalls.Count > 0)
            {
                Ball closestBall = CloseBalls[0];
                float distance = closestBall.Position.DistanceTo(StandOwner.Position);
                for (int i = 1; i < CloseBalls.Count; i++)
                {
                    if (CloseBalls[i].IsInGroup(TeamName)) continue;

                    float newDistance = CloseBalls[i].Position.DistanceTo(StandOwner.Position);
                    if (newDistance < distance)
                    {
                        closestBall = CloseBalls[i];
                        distance = newDistance;
                    }
                }
                ragnarokPos = closestBall.Position;
            }

            SpawnRagnarok(ragnarokPos);

            RagnarokCooldown = MaxRagnarokCooldown;
        } else
        {
            RagnarokCooldown -= delta;
        }

        MoveToOwner(new Vector2(-80, -128));
    }

    public void SpawnRagnarok(Vector2 position)
    {
        Ragnarok ragnarok = RagnarokScene.Instantiate<Ragnarok>();
        ragnarok.Position = position;
        ragnarok.OwnerSetup(StandOwner);

        EmitSignal(SignalName.SpawnProjectile, ragnarok);
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
