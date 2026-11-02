using Godot;
using System;

public partial class DamageIndicator : Label
{
    public double timer = 0.5;
    public Color modulateColor = Colors.White;

    public override void _Process(double delta)
    {
        Position += new Vector2(0, -10 * (float)delta);

        if (timer > 0)
        {
            timer -= delta;
        } else
        {
            modulateColor.A -= (float)delta * 2;
            if (modulateColor.A <= 0)
            {
                QueueFree();
            }
        }

        Modulate = modulateColor;
    }
}
