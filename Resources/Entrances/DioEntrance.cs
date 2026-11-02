using Godot;
using System;

public partial class DioEntrance : EntranceData
{
    public override void InitialSetup(Ball target)
    {
        if (target is Dio dio)
        {
            dio.Stand.Modulate = Colors.Transparent;
        }
        
        EmitSignal(SignalName.EntranceFinished);
    }
}
