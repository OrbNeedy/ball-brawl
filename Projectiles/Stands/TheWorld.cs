using Godot;
using System;
using System.Collections.Generic;

public partial class TheWorld : Stand
{
    [Export]
    public AudioStream HitSound;
    [Export]
    public AudioStream TimeStopSoundEffect;
    [Export]
    public Shader TimeStopShader;
    public List<Ball> CloseBalls = new();
    [Export]
    public double MaxTimeStopDuration = 5.0;
    public double TimeStopDuration = 0.0;
    [Export]
    public double MaxTimeStopCooldown = 16.0;
    public double TimeStopCooldown = 0;

    public override void OnStandReady()
    {
        TimeStopDuration = 0;
        TimeStopCooldown = 1;
    }

    public override void StandOffProcess(double delta)
    {
        if (TimeStopCooldown > 0 && !TimeStopped)
        {
            TimeStopCooldown -= delta * 0.5;
        }
    }

    public override void StandOutProcess(double delta)
    {
        TimeResistant = !StandOwner.AffectedByTimeStop;
        if (StandOwner.AffectedByTimeStop)
        {
            Velocity = Vector2.Zero;
            return;
        }
        
        if (TimeStopCooldown <= 0)
        {
            GlobalTimeState.Instance.SetAllShaders(null);
            GlobalTimeState.Instance.SetGlobalShader(TimeStopShader);
            GlobalTimeState.Instance.StopTime(MaxTimeStopDuration, TimeStopType.AddToCount);
            TimeStopDuration = MaxTimeStopDuration;
            TimeStopCooldown = MaxTimeStopCooldown;

            SoundEffectManager.Instance.PlaySound(TimeStopSoundEffect, 0.4f, true);
        }

        if (TimeStopDuration > 0)
        {
            StandOwner.TimeResistance += delta;
            TimeStopDuration -= delta;
        }

        if (TimeStopCooldown > 0 && !TimeStopped)
        {
            TimeStopCooldown -= delta;
        }

        if (CloseBalls.Count > 0)
        {
            Ball closestBall = CloseBalls[0];
            float distance = closestBall.Position.DistanceTo(StandOwner.Position);
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

            if (distance <= Range && Position.DistanceTo(StandOwner.Position) <= Range)
            {
                Velocity = Position.DirectionTo(closestBall.Position) * 400;
            } else
            {
                MoveToOwner(new Vector2(-40, -64));
            }
        } else
        {
            MoveToOwner(new Vector2(-40, -64));
        }
    }

    public override void OnBallHit(Ball ball)
    {
        SoundEffectManager.Instance.PlaySound(HitSound, Position, 0.8f, true);
        if (ball.AffectedByTimeStop)
        {
            ball.OldLinearVelocity += Position.DirectionTo(ball.Position) * 200;
        } else
        {
            ball.ApplyImpulse(StandOwner.Position.DirectionTo(ball.Position) * 600);
            //ball.LinearVelocity += Position.DirectionTo(ball.Position) * 300;
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
