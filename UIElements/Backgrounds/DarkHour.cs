using Godot;
using System;

public partial class DarkHour : Background
{
    [Export]
    public Parallax2D Far;
    [Export]
    public Parallax2D Mid;
    [Export]
    public Parallax2D Close;

    public override void _Process(double delta)
    {
        if (GlobalTimeState.Instance.TimeStopDuration > 0)
        {
            Far.Autoscroll = Vector2.Zero;
            Mid.Autoscroll = Vector2.Zero;
            Close.Autoscroll = Vector2.Zero;
        } else
        {
            Far.Autoscroll = new Vector2(10, 0);
            Mid.Autoscroll = new Vector2(35, 0);
            Close.Autoscroll = new Vector2(60, 0);
        }
    }
}
