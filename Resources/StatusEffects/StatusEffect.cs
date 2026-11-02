using Godot;
using System;
using System.Linq;

public enum StatusEffectCategory
{
    None, 
    Heal, 
    Fire, 
    Poison, 
    Damage
}

[GlobalClass]
public partial class StatusEffect : Resource
{
    [Export]
    public PackedScene Particles;
    [Export]
    public double MaxTick = 1;
    [Export]
    public double Tick = 1;
    [Export]
    public bool ResistsTime = false;
    [Export]
    public double MaxTimeLeft = 5;
    [Export]
    public double TimeLeft = 5;

    [Export]
    public int EffectPriority = 0;

    [Export]
    public virtual StatusEffectCategory Category { get; set; }
    [Export]
    public int Stacks = 0;

    public virtual void OnInflict(Ball ball)
    {
        
    }

    public virtual void OnReInflict(Ball ball)
    {
        
    }

    public void Update(double delta, Ball ball)
    {
        if (!ResistsTime && ball.AffectedByTimeStop) return;

        if (Tick <= 0)
        {
            OnTickUpdate(ball);
            Tick = MaxTick;
        } else
        {
            Tick -= delta;
        }

        if (TimeLeft > 0)
        {
            TimeLeft -= delta;
        }
    }

    public virtual void OnTickUpdate(Ball ball)
    {
        
    }
}
