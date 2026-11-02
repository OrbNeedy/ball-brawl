using Godot;
using System;

public partial class TagReticle : AnimatedSprite2D
{
    [Export]
    public Node2D FollowTarget;
    public int level = 0;

    // Tag 1: 00eaff
    // Tag 2: ffff33
    // Tag 3: e6005c

    public override void _Process(double delta)
    {
        if (FollowTarget != null && IsInstanceValid(FollowTarget))
        {
            Visible = true;
            Position = FollowTarget.Position;
        } else
        {
            Visible = false;
            QueueFree();
        }

        if (GlobalTimeState.Instance.TimeStopDuration <= 0)
        {
            SpeedScale = 1;
        } else
        {
            SpeedScale = 0;
        }

        if (level == 0)
        {
            Rotation = 0;
        }
        if (level == 1)
        {
            Rotate((float)delta * SpeedScale);
        }
        if (level == 2)
        {
            Rotate((float)delta * -2 * SpeedScale);
        }
    }

    public void SetTag(Node2D node, int tagLevel)
    {
        FollowTarget = node;
        level = tagLevel;
        Frame = level;

        switch (level)
        {
            case 1:
                Modulate = new Color("ffff33");
                break;
            case 2:
                Modulate = new Color("e6005c");
                break;
            default:
                Modulate = new Color("00eaff");
                break;
        }
    }
}
