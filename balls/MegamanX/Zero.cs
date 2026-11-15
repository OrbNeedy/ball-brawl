using Godot;
using System;

public partial class Zero : Ball
{
    [Export]
    public ZSaber ZSaber;
    [Export]
    public PinJoint2D Joint;

    [Export]
    public double MaxDashTimer = 6;
    [Export]
    public double DashTimer = 6;
    public double DashingLeft = 0;
    public Vector2 LastDash = Vector2.Zero;

    public override void OnProcess(double delta)
    {
        if (DashTimer > 0 && DashingLeft <= 0)
        {
            DashTimer -= delta;
        }
        if (DashingLeft > 0)
        {
            DashingLeft -= delta;
        }
        
        Joint.MotorEnabled = ZSaber.DeflectionMode || DashingLeft > 0;
    }

    public override void OnPhysics(double delta)
    {
        if (DashingLeft > 0)
        {
            //ApplyCentralForce(LastDash * 1600);
        }

        if (DashTimer <= 0)
        {
            Ball closest = GetClosestBall();

            Vector2 direction = Vector2.Up.Rotated(Main.rng.RandfRange(0, (float)Math.Tau));
            if (closest != null && IsInstanceValid(closest))
            {
                direction = Position.DirectionTo(closest.Position);
            }

            LastDash = direction;
            LinearVelocity = Vector2.Zero;
            //ApplyTorqueImpulse(600);
            //Vector2 rotImpulse = ZSaber.Position.Rotated((float)(Math.PI / 2f)).Normalized();
            ApplyCentralImpulse(LastDash * 1200);

            DashTimer = MaxDashTimer;
            DashingLeft = 1;
        }
    }

    public override DamageModification ModifyWeaponDamage(Weapon weapon, DamageData data)
    {
        if (DashingLeft > 0)
        {
            return new(0, -0.20f);
        }
        
        return base.ModifyWeaponDamage(weapon, data);
    }

    public override DamageModification ModifyContactDamage(Ball ball, DamageData data)
    {
        if (DashingLeft > 0)
        {
            return new(0, -0.20f);
        }

        return base.ModifyContactDamage(ball, data);
    }

    public override DamageModification ModifyDealtContactDamage(Ball ball)
    {
        if (DashingLeft > 0)
        {
            return new(3);
        }

        return base.ModifyDealtContactDamage(ball);
    }
}
