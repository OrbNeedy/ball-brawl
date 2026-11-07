using Godot;
using System;

public partial class Marisa : Ball
{
    [Signal]
    public delegate void OnSpawnOreryEventHandler();

    [Export]
    public PackedScene LaserScene;
    [Export]
    public AudioStream LaserSound;
    [Export]
    public OreryOrchestrator Orchestrator;
    [Export]
    public double MaxBallTime = 30;
    [Export]
    public double InitialBallTime = 12;
    public double BallTime = 0;

    [Export]
    public double MaxLaserTime = 3;
    public double LaserTime = 3;

    public override void OnReady()
    {
        BallTime = InitialBallTime;
        LaserTime = MaxLaserTime;
    }

    public override void OnProcess(double delta)
    {
        if (BallTime <= 0)
        {
            if (!Orchestrator.HasOreries)
            {
                EmitSignal(SignalName.OnSpawnOrery);
                BallTime = MaxBallTime;
            }
        } else
        {
            BallTime -= delta;
        }

        if (!Orchestrator.HasOreries)
        {
            if (LaserTime <= 0)
            {
                Ball closest = GetClosestBall();
                Laser laser = LaserScene.Instantiate<Laser>();
                laser.TimeLeft = 1.2;
                laser.OwnerSetup(this);
                if (closest == null || !IsInstanceValid(closest))
                {
                    laser.Velocity = new Vector2(1, 0).Rotated(Main.rng.RandfRange(0, (float)Math.Tau));
                } else
                {
                    laser.Velocity = Position.DirectionTo(closest.Position);
                }
                SoundEffectManager.Instance.PlaySound(LaserSound, Position, 0.3f);

                AddChild(laser);

                LaserTime = MaxLaserTime;
            } else
            {
                LaserTime -= delta;
            }
        }
    }
}
