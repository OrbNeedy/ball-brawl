using Godot;
using System;

public enum WeaponClashType
{
    None, 
    HBB
}

public partial class Weapon : RigidBody2D
{
    [Export]
    public DamageData Damage;
    [Export]
    public WeaponClashType ClashType = WeaponClashType.None;
    [Export]
    public Ball Holder;
    [Export]
    public bool TimeResistant = false;
    public bool TimeStopped { get => GlobalTimeState.Instance.TimeStopDuration > 0; }
    public String TeamName = "";
    public bool Active = true;
    [Export]
    public uint InitialLayer = 1;
    [Export]
    public uint InitialMask = 1;
    
    [Export]
    public bool CanBeDodged = true;
    [Export]
    public bool StoppableByFlashfield = false;
    [Export]
    public bool Almighty = false;
    [Export]
    public AttackElement Element = AttackElement.None;

    Vector2 OldLinearVelocity = Vector2.Zero;

    public override void _Ready()
    {
        if (Holder != null)
        {
            if (Holder.TeamName == "")
            {
                AddToGroup(Holder.BallName);
                TeamName = Holder.BallName;
            } else
            {
                AddToGroup(Holder.TeamName);
                TeamName = Holder.TeamName;
            }
        }

        OnReady();
    }

    public virtual void OnReady()
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Holder != null)
        {
            if (Holder.Freeze)
            {
                if (!Freeze)
                {
                    SetFreeze(true);
                }
            } else
            {
                if (Freeze)
                {
                    SetFreeze(false);
                }
            }
        }

        OnPhysicsProcess(delta);
    }

    public virtual void OnPhysicsProcess(double delta)
    {
        
    }

    public override void _Process(double delta)
    {
        //GD.Print("Damage: " + Damage.BaseDamage);
        if (TimeStopped && !TimeResistant)
        {
            return;
        }

        if (!Active) return;
        OnProcess(delta);
    }

    public virtual void OnProcess(double delta)
    {
    }

    public void OnBodyEntered(Node body) 
    {
        //GD.Print("Damage: " + Damage.BaseDamage);
        if (!Active) return;

        if (body is Weapon clashing)
        {
            OnWeaponClash(clashing);

            if (ClashType == WeaponClashType.HBB || clashing.ClashType == WeaponClashType.HBB)
            {
                ApplyTorqueImpulse(clashing.AngularVelocity * 0.8f);
            }
        }

        if (body is Ball ball)
        {
            OnBallHit(ball);
        }
    }

    public virtual void OnWeaponClash(Weapon other)
    {
        
    }

    public virtual int GetWeaponDamage(Ball ball)
    {
        return Damage.BaseDamage;
    }

    public virtual void OnBallHit(Ball ball)
    {
    }

    public void SetFreeze(bool freeze)
    {
        Freeze = freeze;
        if (freeze)
        {
            OldLinearVelocity = LinearVelocity;
        } else
        {
            LinearVelocity = OldLinearVelocity;
        }
    }

    public void Deactivate()
    {
        Visible = false;

        Active = false;

        CollisionMask = 0;
        CollisionLayer = 0;
    }

    public void Activate()
    {
        Visible = true;

        Active = true;

        CollisionMask = InitialMask;
        CollisionLayer = InitialLayer;
    }
}
