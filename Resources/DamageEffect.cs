using Godot;
using System;

public partial class DamageEffect : Resource
{
    [Export]
    public int DamageFrames = 1;
    [Export]
    public AudioStream DamageSound;
    [Export]
    public AudioStream DeathSound;

    public virtual void TakeDamage(Ball ball, int damage)
    {
        float volume = 1;
        if (damage <= 1)
        {
            volume = 0.15f;
        }
        if (damage <= 4)
        {
            volume = 0.4f;
        }

        if (ball.life <= 0)
        {
            SoundEffectManager.Instance.PlaySound(DeathSound, ball.GlobalPosition, volume);
        } else {
            SoundEffectManager.Instance.PlaySound(DamageSound, ball.GlobalPosition, volume);
        }
    }
}
