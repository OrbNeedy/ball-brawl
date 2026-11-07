using Godot;
using System;

public partial class StatusEffect : Node2D
{
    [Signal]
    public delegate void StatusOverEventHandler(StatusEffect effect);
    public Ball Target;

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
    public virtual String Category { get; set; }
    [Export]
    public int Stacks = 0;

    /// <summary>
    /// When a new status effect is inflicted upon a ball.
    /// </summary>
    /// <param name="ball"></param>
    public void Inflict(Ball ball)
    {
        ball.AddChild(this);

        Target = ball;
        OnInflict(ball);
    }

    /// <summary>
    /// When the same status effect is inflicted upon a ball already with the same category. <br/>
    /// Only happens when the old status has higher priority and the same category as the new effect.
    /// </summary>
    /// <param name="ball"></param>
    public void Reinflict(Ball ball, StatusEffect newStatus)
    {
        OnReinflict(ball, newStatus);

        TimeLeft = MaxTimeLeft;
        Stacks += newStatus.Stacks + 1;
    }

    /// <summary>
    /// When this effect replaces one of the same category on a ball. <br/>
    /// Only happens when the new status has higher priority and the same category as the old effect.
    /// </summary>
    /// <param name="ball"></param>
    /// <param name="oldStatus"></param>
    public void Replace(Ball ball, StatusEffect oldStatus)
    {
        ball.AddChild(this);

        Target = ball;
        Stacks += oldStatus.Stacks + 1;
    }

    public void Replaced(Ball ball)
    {
        TimeLeft = 0;
    }

    public virtual void OnInflict(Ball ball)
    {
        
    }

    public virtual void OnReinflict(Ball ball, StatusEffect newStatus)
    {
        
    }

    public virtual void OnReplace(Ball ball, StatusEffect oldStatus)
    {
        
    }

    public override void _Process(double delta)
    {
        Update(delta);

        if (Target == null || !IsInstanceValid(Target))
        {
            QueueFree();
            EmitSignal(SignalName.StatusOver);
            return;
        }

        if (!ResistsTime && Target.AffectedByTimeStop) return;

        if (TimeLeft > 0)
        {
            if (Tick <= 0)
            {
                OnTickUpdate(Target);
                Tick = MaxTick;
            } else
            {
                Tick -= delta;
            }

            TimeLeft -= delta;
            if (TimeLeft <= 0)
            {
                EmitSignal(SignalName.StatusOver, this);
            }
        } else
        {
            if (ReadyToFree()) QueueFree();
        }
    }

    /// <summary>
    /// General update tasks, executed before the target is verified and every other process task.
    /// </summary>
    /// <param name="delta"></param>
    public virtual void Update(double delta)
    {
        
    }

    public virtual bool ReadyToFree()
    {
        return true;
    }
    
    public virtual DamageModification ProjectileDamageModification(Ball ball, Projectile projectile)
    {
        return new();
    }
    
    public virtual DamageModification ContactDamageModification(Ball ball, Ball other)
    {
        return new();
    }
    
    public virtual DamageModification WeaponDamageModification(Ball ball, Weapon weapon)
    {
        return new();
    }
    
    public virtual DamageModification FinalDamageModification(Ball ball)
    {
        return new();
    }
    
    public virtual DamageModification DealtDamageModification(Ball ball, Ball other)
    {
        return new();
    }

    public virtual void OnTickUpdate(Ball ball)
    {
        
    }
}
