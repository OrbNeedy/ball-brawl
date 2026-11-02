using Godot;
using Godot.Collections;
using System;

public partial class HealthUi : Control
{
    [Export]
    public Control LifeBarContainer;
    [Export]
    public Control DamageIndicatorContainer;
    [Export]
    public Array<Ball> TargetBalls = new();
    [Export]
    public PackedScene Bar;
    [Export]
    public PackedScene Indicator;
    [Export]
    public Marker2D center;
    [Export]
    public Vector2 BattleViewportPosition = Vector2.Zero;

    public override void _Ready()
    {
    }

    public void SetBars()
    {
        float halfMeasure = 50f;

        float min = halfMeasure * (-TargetBalls.Count + 1);
        float max = halfMeasure * (TargetBalls.Count - 1);
        
        // Due to Control node jank, subtract initially
        float offsetX = min - halfMeasure;
        int count = 0;
        //GD.Print("Min: " + min);
        foreach (Ball ball in TargetBalls)
        {
            Lifebar lifebar = Bar.Instantiate<Lifebar>();
            ball.DamageTaken += lifebar.UpdateLife;
            ball.DamageTaken += OnTakenDamage;
            ball.BallDead += lifebar.BallDead;
            float barY = 20;

            lifebar.SyncBallIdentity(ball);
            
            if (count % 2 == 0)
            {
                barY = 70;
            }

            //GD.Print("Offset X: " + offsetX);

            lifebar.Position = new Vector2(center.Position.X + offsetX, barY);
            LifeBarContainer.AddChild(lifebar);
            offsetX += halfMeasure * 2;
            count++;
        }
    }

    public void AddBallPreFight(Ball ball)
    {
        if (TargetBalls.Contains(ball)) return;

        TargetBalls.Add(ball);
    }

    public void AddBallMidFight(Ball ball)
    {
        if (TargetBalls.Contains(ball)) return;
    }

    public void OnTakenDamage(float percent, int damageTaken, Ball ball)
    {
        DamageIndicator damageIndicator = Indicator.Instantiate<DamageIndicator>();

        damageIndicator.modulateColor = ball.Color;
        string sign = "-";
        if (damageTaken < 0)
        {
            sign = "+";
        }
        damageIndicator.Text = sign + int.Abs(damageTaken).ToString();
        Vector2 offset = new Vector2(Main.rng.RandfRange(-50, 50), Main.rng.RandfRange(-50, 50));
        damageIndicator.Position = BattleViewportPosition + ball.Position - (damageIndicator.Size / 2f) + offset;

        DamageIndicatorContainer.AddChild(damageIndicator);
    }
}
