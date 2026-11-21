using Godot;
using System;

public partial class Background : Control
{
    [Export]
    public SubViewport BallsViewport;
    [Export]
    public SubViewport ProjectilesViewport;

    public void OnReceiveSignal(string signal)
    {
        OnReceiveSignal(signal, new float[0]);
    }

    public virtual void OnReceiveSignal(string signal, params float[] args)
    {
        switch (signal)
        {
            default:
                GD.Print($"Signal {signal} received but not recognized.");
                break;
        }
    }
}
