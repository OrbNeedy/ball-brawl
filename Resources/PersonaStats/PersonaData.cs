using Godot;
using System;

public partial class PersonaData : Resource
{
    [Export]
    public PackedScene Persona;
    [Export]
    public PowerType Power;
    [Export]
    public int ContactDamage = 0;
    [Export]
    public float ContactDamageMultiplicative = 0;
    [Export]
    public int ContactDamageReduction = 0;
    [Export]
    public float ContactDamageMultiplicativeReduction = 0;
    [Export]
    public int ProjectileDamageReduction = 0;
    [Export]
    public float ProjectileDamageMultiplicativeReduction = 0;
    [Export]
    public int WeaponDamageReduction = 0;
    [Export]
    public float WeaponDamageMultiplicativeReduction = 0;
    [Export]
    public int DamageReduction = 0;
    [Export]
    public float DamageMultiplicativeReduction = 0;
    [Export]
    public float SpeedGainModification = 0;
    [Export]
    public float MaxSpeedGainModification = 0;
    [Export]
    public double MaxTimeStopResistanceModification = 0;
}
