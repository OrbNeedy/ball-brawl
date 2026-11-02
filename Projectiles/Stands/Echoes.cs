using Godot;
using System;

public partial class Echoes : Stand
{
    [Export]
    public Shape2D[] ActShapes;

    [Export]
    public double MaxModeChangeTimer = 18;
    [Export]
    public double ModeChangeTimer = 10;

    [Export]
    public int Stage = 0;
    public bool Grounded = false;

    public override void StandOutProcess(double delta)
    {
        switch (Stage)
        {
            case 0:
                if (!Grounded)
                {
                    Velocity = Velocity.Lerp(new Vector2(0, 500), 0.08f);
                }
                break;
            case 1:
                MoveToOwner(new Vector2(-40, -64));
                break;
            case 2:
                MoveToOwner(new Vector2(-40, -64));
                break;
            case 3:
                MoveToOwner(new Vector2(-40, -64));
                break;
        }

        if (ModeChangeTimer <= 0)
        {
            IncreaseStage();
            ModeChangeTimer = MaxModeChangeTimer;
        } else
        {
            ModeChangeTimer -= delta;
        }
    }

    public void IncreaseStage()
    {
        Stage = int.Clamp(Stage + 1, 0, 3);

        body.Animation = "act" + Stage;
        GetNode<CollisionShape2D>("CollisionShape2D").Shape = ActShapes[Stage];
    }

    public void OwnerDamageTaken(float newLifePercent, int damageTaken, Ball ball)
    {
        ModeChangeTimer -= damageTaken * 0.25;
    }
    
    public void OnLowerBorderCollision(Node2D body)
    {
        if (body.IsInGroup("Boundary"))
        {
            Grounded = true;
            Velocity = Vector2.Zero;
        }
    }
}
