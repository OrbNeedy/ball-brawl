using Godot;
using System;

public partial class TouhouDamageStyle : DamageEffect
{
    public override void TakeDamage(Ball ball, int damage)
    {
        if (DamageFrames <= 0) return;

        float lifePercent = (float)ball.life / (float)ball.MaxLife;
        float lifePerFrame = 1f / (float)DamageFrames;
        
        ball.Body.Frame = DamageFrames - (int)float.Floor(lifePercent / lifePerFrame) - 1;

        base.TakeDamage(ball, damage);
    }
}
