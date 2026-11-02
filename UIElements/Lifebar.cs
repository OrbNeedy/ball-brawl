using Godot;
using System;

public partial class Lifebar : TextureProgressBar
{
    [Export]
    public Label NameLabel;
    [Export]
    public Label LifeLabel;

    public void SyncBallIdentity(Ball ball)
    {
        SelfModulate = ball.Color;
        if (NameLabel != null)
        {
            NameLabel.Text = ball.BallName;
        }
    }

    public override void _Process(double delta)
    {
    }

    public void UpdateLife(float percent, int damageTaken, Ball ball)
    {
        Value = MaxValue * percent;
        LifeLabel.Text = Value.ToString();
    }

    public void BallDead(Ball ball)
    {
        QueueFree();
    }
}
