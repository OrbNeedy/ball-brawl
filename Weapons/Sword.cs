using Godot;
using System;

public partial class Sword : Weapon
{
    [Export]
    public int MaxSpeedDamage = 5;
    [Export]
    public float SpeedDamageIncrease = 100f;

    public override DamageModification GetWeaponDamageModifications(Ball ball)
    {
        int bonus = int.Clamp((int)(LinearVelocity.Length() / SpeedDamageIncrease), 0, MaxSpeedDamage);

        return new(bonus);
    }
}
