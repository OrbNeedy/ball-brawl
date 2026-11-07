using Godot;
using System;

public partial class Sizzle : Projectile
{
    public Color InactiveColor = new(0.133f, 0.002f, 0.018f, 0.75f);
    public Color SizzlingColor = new(1f, 0.006f, 0.1f, 1f);
    public double ContactTimer = 0;

    public override void OnProcess(double delta)
    {
        if (ballsHit.Count > 0)
        {
            ContactTimer += delta * 2;
        } else
        {
            ContactTimer -= delta;
        }

        ContactTimer = double.Clamp(ContactTimer, 0, 1);

        body.Modulate = InactiveColor.Lerp(SizzlingColor, (float)ContactTimer);
    }
}
