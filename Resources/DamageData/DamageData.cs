using Godot;
using System;

[GlobalClass]
public partial class DamageData : Resource
{
    [Export]
    public int BaseDamage = 0;
    [Export]
    public double IFrames = 0.2;
    [Export]
    public AttackElement Element = AttackElement.None;
    [Export]
    public bool CanBeDodged = true;
    [Export]
    public bool Almighty = false;
}
