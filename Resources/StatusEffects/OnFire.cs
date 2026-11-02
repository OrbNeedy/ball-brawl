using Godot;
using System;

public partial class OnFire : StatusEffect
{
    [Export]
    public int Damage = 1;

    public override void OnReInflict(Ball ball)
    {
        TimeLeft = MaxTimeLeft;
    }

    public override void OnTickUpdate(Ball ball)
    {
        GD.Print("Status tick update");
        ball.DamageBall(Damage + Stacks, 0);
    }
}
