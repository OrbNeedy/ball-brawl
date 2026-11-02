using Godot;
using System;

public partial class EntranceData : Resource
{
    [Signal]
    public delegate void EntranceFinishedEventHandler();
    [Export]
    public int Priority = 0;

    public virtual void InitialSetup(Ball target)
    {
        EmitSignal(SignalName.EntranceFinished);
    }
}
