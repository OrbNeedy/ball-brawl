using Godot;
using System;
using System.Collections.Generic;

public partial class Cybele : Stand
{
    [Export]
    public AudioStream HealingSound;
    [Export]
    public PackedScene healingParticles;
    [Export]
    public int HealingOthersAmount = 2;
    [Export]
    public int HealingSelfAmount = 8;
    [Export]
    public double MaxHealOthersCooldown = 1;
    public double HealOthersCooldown = 0;
    [Export]
    public double HealSelfCooldown = 1;
    public List<Ball> CloseBalls = new();

    public override void StandOutProcess(double delta)
    {
        if (HealOthersCooldown <= 0)
        {
            for (int i = 1; i < CloseBalls.Count; i++)
            {
                if (CloseBalls[i].IsInGroup(TeamName) && CloseBalls[i] != StandOwner)
                {
                    CloseBalls[i].HealBall(HealingOthersAmount);
                    SoundEffectManager.Instance.PlaySound(HealingSound, CloseBalls[i].Position, 0.8f);

                    SelfDeletingParticles particle = healingParticles.Instantiate<SelfDeletingParticles>();
                    particle.FollowTarget = CloseBalls[i];
                    EmitSignal(SignalName.SpawnParticles, particle);
                }
            }

            HealOthersCooldown = MaxHealOthersCooldown;
        } else
        {
            HealOthersCooldown -= delta;
        }

        if (HealSelfCooldown > 0)
        {
            HealSelfCooldown -= delta;
            if (HealSelfCooldown <= 0)
            {
                StandOwner.HealBall(HealingSelfAmount);
                SoundEffectManager.Instance.PlaySound(HealingSound, StandOwner.Position, 0.8f);

                SelfDeletingParticles particle = healingParticles.Instantiate<SelfDeletingParticles>();
                particle.FollowTarget = StandOwner;
                EmitSignal(SignalName.SpawnParticles, particle);
            }
        }

        MoveToOwner(new Vector2(-40, -64));
    }

    public void OnDetectBall(Node2D body)
    {
        if (!body.IsInGroup(TeamName)) return;

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
