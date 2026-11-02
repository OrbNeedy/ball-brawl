using Godot;
using System;

public partial class Sword : Weapon
{
    [Export]
    public int MaxSpeedDamage = 5;
    [Export]
    public float SpeedDamageIncrease = 100f;

    public override int GetWeaponDamage(Ball ball)
    {
        int bonus = int.Clamp((int)(LinearVelocity.Length() / SpeedDamageIncrease), 0, MaxSpeedDamage);
        return base.GetWeaponDamage(ball) + bonus;
    }
}
