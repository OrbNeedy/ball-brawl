using Godot;
using System;

public partial class Camera3d : Camera3D
{
    [Export]
    public double Time = 0;
    
    public override void _Process(double delta)
    {
        HOffset = (float)Math.Sin(Time) * 0.8f;
        VOffset = (float)Math.Sin(Time * 2) * 0.8f;

        if (GlobalTimeState.Instance.TimeStopDuration > 0) return;

        Time += delta * 0.3;
    }
}
