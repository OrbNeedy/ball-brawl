using Godot;
using System;

public partial class NativeCross : Node3D
{
    [Export]
    MeshInstance3D cross;

    [Export]
    public double MaxTimer = 10;

    public bool Showing = false;
    public double ShowingTimer = 0;

    public override void _Ready()
    {
        (cross.GetSurfaceOverrideMaterial(0) as ShaderMaterial).SetShaderParameter("alpha", 0f);
    }

    public override void _Process(double delta) 
    {
        double transparency = double.Clamp(ShowingTimer / MaxTimer, 0, 1);
        (cross.GetSurfaceOverrideMaterial(0) as ShaderMaterial).SetShaderParameter("alpha", transparency);

        if (GlobalTimeState.Instance.TimeStopDuration > 0) return;

        RotateObjectLocal(new Vector3(0, 1, 0), 1.25f * (float)delta);
        if (Showing)
        {
            ShowingTimer += delta;
        } else
        {
            ShowingTimer -= delta;
        }

        ShowingTimer = double.Clamp(ShowingTimer, 0, MaxTimer);
    }

    public void Pulse()
    {
        ShowingTimer = MaxTimer / 2f;
    }

    public void ShowCross()
    {
        Showing = true;
    }

    public void HideCross()
    {
        Showing = false;
    }
}
