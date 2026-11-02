using Godot;
using System;

public partial class Dio : Ball
{
    [Export]
    public AudioStream StandOut;
    [Export]
    public AudioStream StandOff;
    [Export]
    public double MaxModeChangeTimer = 10;
    public double ModeChangeTimer = 10;
    public bool StandOutMode = false;
    [Export]
    public PackedScene StandScene;
    public TheWorld Stand;
    [Export]
    public Weapon RoadSign;

    public override void OnReady()
    {
        BasicCharacterballInit();
        ModeChangeTimer = MaxModeChangeTimer;
        
        Stand = StandScene.Instantiate<TheWorld>();
        Stand.SetOwner(this);
        Stand.StandOut = StandOutMode;
        EmitSignal(SignalName.SpawnProjectile, Stand);
    }

    // Theme from https://www.youtube.com/watch?v=ukUcvkSgY9k
    // By Astrophysics

    public override void OnProcess(double delta)
    {
        if (ModeChangeTimer <= 0 && !TimeStopped && !Stand.Attacking)
        {
            ModeChangeTimer = MaxModeChangeTimer;
            if (StandOutMode)
            {
                GravityScale = 1f;
                RoadSign.Activate();
                SoundEffectManager.Instance.PlaySound(StandOff, Position, 0.6f, true);
            } else
            {
                GravityScale = 0.15f;
                RoadSign.Deactivate();
                SoundEffectManager.Instance.PlaySound(StandOut, 0.6f, true);
            }
            StandOutMode = !StandOutMode;
            Stand.StandOut = StandOutMode;
        } else
        {
            ModeChangeTimer -= delta;
        }
    }

    public override DamageData GetContactDamage(Ball ball)
    {
        if (StandOutMode)
        {
            DamageData newData = (DamageData)base.GetContactDamage(ball).Duplicate();
            newData.BaseDamage += 2;
            return newData;
        }

        return base.GetContactDamage(ball);
    }
}
