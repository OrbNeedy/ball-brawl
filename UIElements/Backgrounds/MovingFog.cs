using Godot;
using System;

public partial class MovingFog : Sprite3D
{
    [Export]
    public float TimeScale = 0.25f;
    [Export]
    public Vector2 MaxMove = new Vector2(30, 0);
    [Export]
    public Vector2 SineScale = Vector2.One;
    [Export]
    public Vector2 SineOffset = Vector2.Zero;
    [Export]
    public bool UseCosX = false;
    [Export]
    public bool UseCosY = false;
    public double Time = 0;

    public override void _Ready()
    {
        Time = Main.rng.RandfRange(-10, 10);
    }

    public override void _Process(double delta)
    {
        double xTime = (Time + SineOffset.X) * SineScale.X;
        float xOffset = (float)Math.Sin(xTime);
        if (UseCosX)
        {
            xOffset = (float)Math.Cos(xTime);
        }

        double yTime = (Time + SineOffset.Y) * SineScale.Y;
        float yOffset = (float)Math.Sin(yTime);   
        if (UseCosY)
        {
            yOffset = (float)Math.Cos(yTime);
        }
        Offset = new Vector2(xOffset * MaxMove.X, yOffset * MaxMove.Y);
        if (GlobalTimeState.Instance.TimeStopDuration > 0) return;

        Time += delta * TimeScale;
    }
}
