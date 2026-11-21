using Godot;
using System;

public partial class FalseBackground : Control
{
    [Export]
    public Parallax2D parallax;
    [Export]
    public TextureRect blobs;
    [Export]
    public ColorRect cover;

    public bool Transparent = false;
    public double Timer = 0;
    public double MaxTimer = 2;

    public override void _Process(double delta)
    {
        cover.Color = new Color(0f, 0f, 0f, 1 - (float)(Timer / MaxTimer));

        if (GlobalTimeState.Instance.TimeStopDuration > 0) return;

        if (Transparent)
        {
            Timer += delta;
        } else
        {
            Timer -= delta * 2;
        }
    }

    public void Pulse()
    {
        Timer = 0.8;
    }

    public void ShowBehind()
    {
        GD.Print("Receding");
        Transparent = true;
    }

    public void Deactivate()
    {
        Visible = false;
    }
}
