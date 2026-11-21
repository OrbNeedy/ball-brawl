using Godot;
using System;

public partial class Transition : AnimatedSprite2D
{
    [Signal]
    public delegate void TransitionFinishedEventHandler();
    public bool Broken = false;

    public void StartBreaking()
    {
        if (!Broken)
        {
            Play("default");
            Broken = true;
        }
    }

    public void OnAnimationFinished()
    {
        if (Animation == "default")
        {
            EmitSignal(SignalName.TransitionFinished);
            Play("disappear");
        }
    }
}
