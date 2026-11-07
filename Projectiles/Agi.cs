using Godot;
using System;

public partial class Agi : Projectile
{
    [Export]
    public PackedScene OnFireDebuff;

    public override void OnProcessVisuals(double delta)
    {
        if (Velocity.Length() > 0)
        {
            Rotation = Velocity.Angle();
        }
    }

    public override void OnBallHit(Ball ball)
    {
        OnFire debuff = OnFireDebuff.Instantiate<OnFire>();
        debuff.Category = "Fire";
        debuff.MaxTick = 0.5;
        debuff.MaxTimeLeft = 3;
        debuff.EffectPriority = 1;

        ball.AddStatus(debuff);
    }
}
