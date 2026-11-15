using Godot;
using System;

public partial class PalaceBackground : Background
{
    [Export]
    public Parallax2D Background;
    [Export]
    public TextureRect Reflections;

    public override void _Ready()
    {
        (Reflections.Material as ShaderMaterial).SetShaderParameter("Target", BallsViewport.GetTexture());
    }

    public override void _Process(double delta)
    {

        if (GlobalTimeState.Instance.TimeStopDuration > 0)
        {
            Background.Autoscroll = Vector2.Zero;
        } else
        {
            Background.Autoscroll = new Vector2(0, 16);
        }
    }
}
