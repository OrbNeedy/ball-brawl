using Godot;
using System;

public partial class Cut : Projectile
{
    [Export]
    public int Color = 0;
    [Export]
    public double Delay = 0;
    [Export]
    public float LengthMultiplier = 1;
    public bool deployed = true;

    [Export]
    public AudioStream PreparationSound;
    [Export]
    public AudioStream CutSound;

    public override void OnReady()
    {
        body.Frame = int.Clamp(Color, 0, 7);

        if (Delay > 0)
        {
            Monitoring = false;
            Monitorable = false;
            deployed = false;
            Scale = new Vector2(1, LengthMultiplier);
            SoundEffectManager.Instance.PlaySound(PreparationSound, Position, 0.5f);
        }
    }

    public override void OnProcess(double delta)
    {
        if (TimeLeft <= 0.75)
        {
            if (Monitoring || Monitorable)
            {
                Monitoring = false;
                Monitorable = false;
            }

            float decrease = (float)(4 * delta);
            body.Scale = new Vector2(body.Scale.X - decrease, 1);
            if (body.Scale.X <= 0)
            {
                DeleteProjectile();
            }
            return;
        }

        if (Delay <= 0)
        {
            if (!Monitoring || !Monitorable)
            {
                Monitoring = true;
                Monitorable = true;
                deployed = true;
                SoundEffectManager.Instance.PlaySound(CutSound, Position, 0.5f);
            }

            if (body.Scale.X < 1)
            {
                float increase = (float)(4 * delta);
                body.Scale = new Vector2(body.Scale.X + increase, 1);
            } else
            {
                body.Scale = new Vector2(1, 1);
            }
        } else
        {
            Delay -= delta;
        }
    }
}
