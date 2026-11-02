using Godot;
using System;

public partial class SelfDeletingParticles : CpuParticles2D
{
    public Node2D FollowTarget;
    public Vector2 OffsetToTarget = Vector2.Zero;
    public bool DieWithTarget = false;
    [Export]
    public double TimeLeft = 0;

    public override void _EnterTree()
    {
        //GlobalTimeState.Instance.OnTimeStop += SetProcessFalse;
        //GlobalTimeState.Instance.OnTimeResume += SetProcessTrue;
        Emitting = true;
    }

    public override void _Process(double delta)
    {
        if (GlobalTimeState.Instance.TimeStopDuration > 0)
        {
            SpeedScale = 0;
        } else
        {
            SpeedScale = 1;
        }

        if (FollowTarget != null && IsInstanceValid(FollowTarget))
        {
            Rotation = FollowTarget.Rotation;
            Position = FollowTarget.Position + OffsetToTarget.Rotated(Rotation);
        } else
        {
            FollowTarget = null;
            if (DieWithTarget)
            {
                Emitting = false;
            }
        }

        if (!OneShot)
        {
            TimeLeft -= delta;
            if (TimeLeft <= 0 && !DieWithTarget)
            {
                Emitting = false;
            }
        }
    }

    public void ParticleFinished()
    {
        //GlobalTimeState.Instance.OnTimeStop -= SetProcessFalse;
        //GlobalTimeState.Instance.OnTimeResume -= SetProcessTrue;
        QueueFree();
    }

    public void SetProcessFalse()
    {
        //ProcessMode = ProcessModeEnum.Disabled;
    }

    public void SetProcessTrue()
    {
        //ProcessMode = ProcessModeEnum.Inherit;
    }
}
