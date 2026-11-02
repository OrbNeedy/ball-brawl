using Godot;
using System;

public partial class BetterCross : Node3D
{
    [Export]
    public float XRotation = 0;
    [Export]
    public float YRotation = 0;
    [Export]
    public float ZRotation = 0;

    public override void _Process(double delta)
    {
        RotateX((float)delta * XRotation);
        RotateY((float)delta * YRotation);
        RotateZ((float)delta * ZRotation);
    }
}
