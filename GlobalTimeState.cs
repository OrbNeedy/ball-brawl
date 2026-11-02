using Godot;
using System;

public enum TimeStopType {
    BiggestOnly, 
    AddToCount, 
    IgnoreIfExisting, 
    OverrideExisting
}

public partial class GlobalTimeState : Node
{
    [Signal]
    public delegate void OnTimeStopEventHandler();
    [Signal]
    public delegate void OnTimeResumeEventHandler();
    public static GlobalTimeState Instance { get; private set; }
    [Export]
    public Shader BackgroundTimeStopShader = null;
    [Export]
    public Shader BallsTimeStopShader = null;
    [Export]
    public Shader GlobalTimeStopShader = null;
    // Time scale might not work
    public float TimeScale = 1;
    public double ModificationDuration = 0;
    public double TimeStopDuration = 0;

    public override void _Ready()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        if (ModificationDuration <= 0)
        {
            TimeScale = 1;
        } else
        {
            ModificationDuration -= delta;
        }
        
        if (TimeStopDuration > 0)
        {
            TimeStopDuration -= delta;
            if (TimeStopDuration <= 0)
            {
                EmitSignal(SignalName.OnTimeResume);
            }
        }
    }

    public void SetTimeScale(float scale, double duration)
    {
        TimeScale = scale < 0 ? float.MinValue : scale;
        ModificationDuration = duration;
    }

    public void SetBackgroundShader(Shader shader)
    {
        BackgroundTimeStopShader = shader;
    }

    public void SetBallsShader(Shader shader)
    {
        BallsTimeStopShader = shader;
    }

    public void SetGlobalShader(Shader shader)
    {
        GlobalTimeStopShader = shader;
    }

    public void SetAllShaders(Shader shader)
    {
        BackgroundTimeStopShader = shader;
        BallsTimeStopShader = shader;
        GlobalTimeStopShader = shader;
    }

    public void StopTime(double duration, TimeStopType stopType = TimeStopType.BiggestOnly)
    {
        EmitSignal(SignalName.OnTimeStop);
        switch (stopType)
        {
            case TimeStopType.BiggestOnly:
                if (duration > TimeStopDuration)
                {
                    TimeStopDuration = duration;
                }
                break;
            case TimeStopType.AddToCount:
                TimeStopDuration += duration;
                break;
            case TimeStopType.IgnoreIfExisting:
                if (TimeStopDuration <= 0)
                {
                    TimeStopDuration = duration;
                }
                break;
            case TimeStopType.OverrideExisting:
                TimeStopDuration = duration;
                break;
        }
    }
}
