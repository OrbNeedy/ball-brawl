using Godot;
using System;

public partial class EchoesSoundEffect : StatusEffect
{
    [Export]
    public AnimatedSprite2D Sound;

    public override void Update(double delta)
    {
        float sine = (float)(Math.Sin(TimeLeft * 0.3f) + 1) * 0.5f * 0.25f;
        Sound.Modulate = new(Sound.Modulate.R, Sound.Modulate.G, Sound.Modulate.B, 0.5f + sine);
    }

    public override DamageModification DealtDamageModification(Ball ball, Ball other)
    {
        float extra = Stacks * 0.025f;
        return new(-1, -extra);
    }

    public override DamageModification FinalDamageModification(Ball ball)
    {
        int extra = (int)float.Floor(Stacks / 8f);
        return new(extra);
    }
}