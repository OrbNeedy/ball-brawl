using Godot;
using System;

public partial class Louis : Ball
{
    [Export]
    public int MaxSpeedDamage = 6;
    [Export]
    public float SpeedDamageIncrease = 80f;

    public override DamageModification ModifyDealtContactDamage(Ball ball)
    {
        int bonus = int.Clamp((int)(LinearVelocity.Length() / SpeedDamageIncrease), 0, MaxSpeedDamage);
        return new(bonus);
    }
}
