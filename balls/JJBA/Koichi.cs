using Godot;
using System;

public partial class Koichi : Ball
{
    [Export]
    public AudioStream StandOut;
    [Export]
    public AudioStream StandOff;
    [Export]
    public PackedScene StandScene;

    [Export]
    public double InitialDeploymentTimer = 3;
    public bool Deployed;
    
    public Echoes Stand;

    public override void OnProcess(double delta)
    {
        if (InitialDeploymentTimer <= 0 && !Deployed)
        {
            Stand = StandScene.Instantiate<Echoes>();
            Stand.SetOwner(this);
            Stand.StandOut = true;
            DamageTaken += Stand.OwnerDamageTaken;
            EmitSignal(SignalName.SpawnProjectile, Stand);

            Deployed = true;
        } else
        {
            InitialDeploymentTimer -= delta;
        }
    }
}
