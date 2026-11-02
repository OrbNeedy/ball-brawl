using Godot;
using System;

public partial class YabusameDamageStyle : DamageEffect
{
    [Export]
    public AudioStream BarrierBreak;
    [Export]
    public AudioStream BarrierRecover;
    [Export]
    public float BarrierThreshold = 0.65f;

    public override void TakeDamage(Ball ball, int damage)
    {
        float lifePercent = (float)ball.life / (float)ball.MaxLife;
        float oldLifePercent = (float)ball.oldLife / (float)ball.MaxLife;
        
        //GD.Print("Old: " + oldLifePercent);
        //GD.Print("Current: " + lifePercent);
        //GD.Print("Barrier: " + BarrierThreshold);
        if (BarrierThreshold > lifePercent)
        {
            float barrierPercent = lifePercent / BarrierThreshold;
            ball.Body.Frame = 1 + (int)((1 - barrierPercent) * (DamageFrames - 1));
            if (oldLifePercent >= BarrierThreshold)
            {
                SoundEffectManager.Instance.PlaySound(BarrierBreak, ball.GlobalPosition);
            }
        } else
        {
            ball.Body.Frame = 0;
            if (oldLifePercent < BarrierThreshold)
            {
                SoundEffectManager.Instance.PlaySound(BarrierRecover, ball.GlobalPosition);
            }
        }

        base.TakeDamage(ball, damage);
    }
}
