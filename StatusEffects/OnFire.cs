using Godot;
using System;

public partial class OnFire : StatusEffect
{
    bool FinishedEmitting = false;
    [Export]
    public SelfDeletingParticles Particles;
    [Export]
    public int DamagePerStack = 1;

    public override void Update(double delta)
    {
        Particles.TimeLeft = TimeLeft;
    }

    public override void OnTickUpdate(Ball ball)
    {
        ball.DamageBallDirect(1 + Stacks, 0);
    }

    public void OnStatusOver(StatusEffect effect)
    {
        Particles.Emitting = false;
        TimeLeft = 0;
    }

    public void OnParticlesOver()
    {
        FinishedEmitting = true;
    }

    public override bool ReadyToFree()
    {
        return FinishedEmitting;
    }
}
