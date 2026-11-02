using Godot;
using System;

public partial class FunctioningAnimatedSprite : AnimatedSprite2D
{
    [Export]
    public bool ResistsTime = false;

    public override void _Process(double delta)
    {
        SpeedScale = 1;
        if (GlobalTimeState.Instance.TimeStopDuration > 0 && !ResistsTime)
        {
            SpeedScale = 0;
        }
    }
}
