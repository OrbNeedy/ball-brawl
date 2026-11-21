using Godot;
using System;

public partial class Gengetsu : Ball
{
    [Export]
    public AudioStream ShootSound;
    
    [Export]
    public PackedScene GengetsuBullets;
    [Export]
    public PackedScene RainingBullets;
    [Export]
    public PackedScene BouncingShines;
    [Export]
    public PackedScene MasterSpark;

    public double AttackTimer = 4;
    
    public double MaxTeleportTime = 1;
    public double TeleportTime = 0;
    public Vector2 OldPos = Vector2.Zero;
    public Vector2 TeleportTarget = Vector2.Zero;
    public Vector2 OldLinearVel = Vector2.Zero;

    public double TeleportCooldown = 8;

    [Export]
    public uint InitialCollisionLayers = 0;
    [Export]
    public uint InitialCollisionMasks = 0;

    public double WaveTimer = 0;

    public bool Phase2Started = false;

    public override void PreProcess(double delta)
    {
        (Body.Material as ShaderMaterial).SetShaderParameter("time", WaveTimer);
        float progress = (float)Math.Sin((TeleportTime / MaxTeleportTime) * Math.PI);
        (Body.Material as ShaderMaterial).SetShaderParameter("progress", progress);
        // GD.Print($"Timer: {WaveTimer} Progress: {progress}");
        // (Body.Material as ShaderMaterial).SetShaderParameter("color", Modulate);
    }

    public override void OnProcess(double delta)
    {
        if (TeleportCooldown <= 0)
        {
            SolicitTeleport();
            TeleportCooldown = 8;
        } else
        {
            TeleportCooldown -= delta;
        }

        if (AttackTimer <= 0)
        {

            AttackTimer = 4;
        } else
        {
            AttackTimer -= delta;
        }
    }

    public void SolicitTeleport()
    {
        Vector2 size = GetViewportRect().Size;
        SolicitTeleport(new Vector2(Main.rng.RandfRange(0, size.X), Main.rng.RandfRange(0, size.Y)));
    }

    public void SolicitTeleport(Vector2 location)
    {
        OldPos = Position;
        OldLinearVel = LinearVelocity;
        TeleportTarget = location;
        TeleportTime = MaxTeleportTime;
    }

    public override void OnPhysics(double delta)
    {
        if (TeleportTime <= 0)
        {
            CollisionLayer = InitialCollisionLayers;
            CollisionMask = InitialCollisionMasks;
        } else
        {
            TeleportTime -= delta;
            if (TeleportTime <= 0)
            {
                LinearVelocity = OldLinearVel;
            }
            WaveTimer += delta;
            CollisionLayer = 0;
            CollisionMask = 0;
        }
    }

    public override void ModifyForcesTransform(ref Transform2D transformation, PhysicsDirectBodyState2D state)
    {
        if (TeleportTime > 0)
        {
            float progress = (float)(1 - (TeleportTime / MaxTeleportTime));
            transformation.Origin = OldPos.Lerp(TeleportTarget, progress);
            state.LinearVelocity = Vector2.Zero;
        }
    }

    public override void OnDamaged(int damage)
    {
        float half = MaxLife / 2f;
        if (oldLife > half && life <= half && !Phase2Started)
        {
            EmitSignal(SignalName.BackgroundSignal, "show", new float[0]);
            Phase2Started = true;
        }
    }
}
