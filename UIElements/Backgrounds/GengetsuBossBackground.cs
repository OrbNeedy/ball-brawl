using Godot;
using System;

public partial class GengetsuBossBackground : Background
{
    [Signal]
    public delegate void MoveToTargetEventHandler();
    [Signal]
    public delegate void BackToZeroEventHandler();
    [Signal]
    public delegate void BrightenEventHandler();
    [Signal]
    public delegate void PulseEventHandler();

    public override void _Process(double delta)
    {
        if (Input.IsActionJustReleased("Accept"))
        {
            //OnReceiveSignal("show");
        }
    }

    public override void OnReceiveSignal(string signal, params float[] args)
    {
        //GD.Print($"Signal {signal} received.");
        switch (signal)
        {
            case "show":
                EmitSignal(SignalName.MoveToTarget);
                break;
            case "hide":
                EmitSignal(SignalName.BackToZero);
                break;
            case "brighten":
                EmitSignal(SignalName.Brighten);
                break;
            case "pulse":
                EmitSignal(SignalName.Pulse);
                break;
            default:
                GD.Print($"Signal {signal} received but not recognized.");
                break;
        }
    }
}
