using Godot;
using System;

public partial class Reticle : AnimatedSprite2D
{
    [Export]
    public Ball BallOwner;
    [Export]
    public AnimatedSprite2D InternalReticle;
    public string TargetAnimation = default;
    public int TargetFrame = 0;

    public override void _Process(double delta)
    {
        Visible = true;
        Animation = TargetAnimation;
        Frame = TargetFrame;
        switch (Animation)
        {
            case "gunvolt":
                GunvoltAnimation(delta);
                break;
            default:
                Visible = false;
                Modulate = Colors.White;
                Rotation = 0;
                break;
        }

        if (GlobalTimeState.Instance.TimeStopDuration > 0) return;

        TargetAnimation = default;
        TargetFrame = 0;
    }

    public void GunvoltAnimation(double delta)
    {
        switch (Frame)
        {
            case 0:
                Rotation = 0;
                Modulate = Colors.White;
                Visible = false;
                break;
            case 1:
                Rotation = 0;
                Modulate = new Color("50ffff");
                break;
            case 2:
                Rotation += (float)Math.PI * (float)delta * 0.6f;
                Modulate = new Color("bebe2e");
                break;
            case 3:
                Rotation += (float)Math.Tau * (float)delta * 0.6f;
                Modulate = new Color("ff356e");
                break;
        }
        InternalReticle.Animation = Animation;
        InternalReticle.Rotation = -Rotation;
        if (!InternalReticle.IsPlaying())
        {
            InternalReticle.Play("gunvolt");
        }
    }
}
