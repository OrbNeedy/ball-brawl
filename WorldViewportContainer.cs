using Godot;
using System;

public partial class WorldViewportContainer : SubViewportContainer
{
    public bool TimeStopped { get => GlobalTimeState.Instance.TimeStopDuration > 0; }
    [Export]
    public SubViewportContainer BackgroundNode;
    [Export]
    public SubViewportContainer BallsNode;

    public override void _Process(double delta)
    {
        if (TimeStopped)
        {
            (BackgroundNode.Material as ShaderMaterial).Shader = GlobalTimeState.Instance.BackgroundTimeStopShader;
            (BallsNode.Material as ShaderMaterial).Shader = GlobalTimeState.Instance.BallsTimeStopShader;
            (Material as ShaderMaterial).Shader = GlobalTimeState.Instance.GlobalTimeStopShader;
        } else
        {
            (BackgroundNode.Material as ShaderMaterial).Shader = null;
            (BallsNode.Material as ShaderMaterial).Shader = null;
            (Material as ShaderMaterial).Shader = null;
        }
    }
}
