using Godot;
using System;

public partial class Megidolaon : Projectile
{
    [Export]
    public PackedScene ParticleScene;
    [Export]
    public AudioStream StartingSound;

    public override void OnReady()
    {
        SelfDeletingParticles particles = ParticleScene.Instantiate<SelfDeletingParticles>();
        particles.FollowTarget = this;
        particles.DieWithTarget = true;
        
        EmitSignal(SignalName.SpawnParticles, particles);

        SoundEffectManager.Instance.PlaySound(StartingSound, Position);
    }
}
