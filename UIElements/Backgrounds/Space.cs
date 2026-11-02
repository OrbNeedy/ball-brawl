using Godot;
using System;

public partial class Space : Background
{
    [Export]
    public Vector2 ScrollSpeed = Vector2.Zero;
    [Export]
    public Parallax2D FarStars;
    [Export]
    public Parallax2D CloseStars;
    [Export]
    public double MinChangeStartTimer = 0;
    [Export]
    public double MaxChangeStartTimer = 12;
    public double ChangeStartTimer = 0;
    [Export]
    public double MinChangeTimer = 1;
    [Export]
    public double MaxChangeTimer = 4;
    public double LastMaxChangeTimer = 0;
    public double ChangeTimer = 0;
    [Export]
    public float MinChangeRotation = 0;
    [Export]
    public float MaxChangeRotation = (float)Math.Tau;
    public float ChangeRotation = 0;
    [Export]
    public float MinChangeSpeed = -400;
    [Export]
    public float MaxChangeSpeed = 400;
    public float LastChangeSpeed = 0;
    public float ChangeSpeed = 0;

    public override void _Ready()
    {
        float speed = Main.rng.RandfRange((float)MinChangeSpeed, (float)MaxChangeSpeed);
        float rotation = Main.rng.RandfRange(0, (float)Math.Tau);
        ScrollSpeed = new Vector2(speed, 0).Rotated(rotation);
        //GD.Print("Initial: " + ScrollSpeed);
    }


    public override void _Process(double delta)
    {
        if (GlobalTimeState.Instance.TimeStopDuration > 0)
        {
            FarStars.Autoscroll = Vector2.Zero;
            CloseStars.Autoscroll = Vector2.Zero;
        } else
        {
            FarStars.Autoscroll = ScrollSpeed * FarStars.ScrollScale;
            CloseStars.Autoscroll = ScrollSpeed * CloseStars.ScrollScale;

            if (ChangeStartTimer <= 0)
            {
                ChangeTimer = 0;
                ChangeStartTimer = Main.rng.RandfRange((float)MinChangeStartTimer, (float)MaxChangeStartTimer);
                ChangeSpeed = Main.rng.RandfRange((float)MinChangeSpeed, (float)MaxChangeSpeed);
                ChangeRotation = Main.rng.RandfRange((float)MinChangeRotation, (float)MaxChangeRotation);
                LastMaxChangeTimer = Main.rng.RandfRange((float)MinChangeTimer, (float)MaxChangeTimer);

            } else
            {
                ChangeStartTimer -= delta;
            }

            if (ChangeTimer < LastMaxChangeTimer)
            {
                ScrollSpeed += ScrollSpeed.Normalized() * ChangeSpeed * (float)delta;
                ScrollSpeed = ScrollSpeed.Rotated(ChangeRotation * (float)delta);

                ChangeTimer += delta;
            }
        }
    }
}
