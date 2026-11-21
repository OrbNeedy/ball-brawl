using Godot;
using System;

public partial class MoonPlanet : Node3D
{
    [Signal]
    public delegate void FinishedRotatingEventHandler();

    [Export]
    public Vector3 InitialRotation = Vector3.Zero;
    [Export]
    public Vector3 TargetRotation = Vector3.Zero;
    [Export]
    public double MaxTimer = 12;

    public bool Showing = false;
    public double ShowingTimer = 0;

    public override void _Process(double delta)
    {
        float normalizedTime = (float)(ShowingTimer / MaxTimer);
        float weight = normalizedTime * normalizedTime * (3.0f - 2.0f * normalizedTime);
        Rotation = InitialRotation.Lerp(TargetRotation, weight);

        if (GlobalTimeState.Instance.TimeStopDuration > 0) return;

        double OldTimer = ShowingTimer;
        if (Showing)
        {
            ShowingTimer += delta;
            if (ShowingTimer >= MaxTimer && OldTimer < MaxTimer)
            {
                EmitSignal(SignalName.FinishedRotating);
            }
        } else
        {
            ShowingTimer -= delta;
        }

        ShowingTimer = double.Clamp(ShowingTimer, 0, MaxTimer);
    }

    public void ShowMoon()
    {
        Showing = true;
    }

    public void HideMoon()
    {
        Showing = false;
    }
}
