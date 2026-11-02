using Godot;
using System;
using System.Collections.Generic;

public partial class OreryOrchestrator : Node2D
{
    [Export]
    public PackedScene OreryScene;
    [Export]
    public AudioStream SpawnSound;

    [Export]
    public Ball BallOwner;
    public OreryBall[] oreries = new OreryBall[4];
    public bool HasOreries { get
        {
            for (int i = 0; i < oreries.Length; i++)
            {
                if (oreries[i] == null || !IsInstanceValid(oreries[i])) continue;
                
                return true;
            }

            return false;
        }
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        //GD.Print($"Draw instruction");
        List<Vector2> points = new();
        for (int i = 0; i < oreries.Length; i++)
        {
            //GD.Print($"Orery {i} null: {oreries[i] == null}");
            //GD.Print($"Orery {i} is valid: {IsInstanceValid(oreries[i])}");
            if (oreries[i] == null || !IsInstanceValid(oreries[i])) continue;
            
            points.Add(oreries[i].Position);
        }
        if (points.Count >= 2)
        {
            points.Add(points[0]);
        }

        if (points.Count > 1)
        {
            //GD.Print($"Drawing Oreries");
            DrawPolyline(points.ToArray(), BallOwner.Color, 2);
        }
    }

    public void CreateOreries()
    {
        if (HasOreries) return;

        for (int i = 0; i < oreries.Length; i++)
        {
            //GD.Print($"Adding orery {i}");
            OreryBall orery = OreryScene.Instantiate<OreryBall>();
            orery.BallColor = i;
            orery.BallOwner = BallOwner;
            BallOwner.AddCollisionExceptionWith(orery);
            oreries[i] = orery;

            BallOwner.AddChild(orery);
        }
        
        SoundEffectManager.Instance.PlaySound(SpawnSound, Position, 0.6f);
    }
}
