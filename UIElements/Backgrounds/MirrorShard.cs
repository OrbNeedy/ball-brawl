using Godot;
using System;

public partial class MirrorShard : Polygon2D
{
    [Export]
    public float FallSpeed = 70;
    [Export]
    public ShardData[] data;

    public override void _EnterTree()
    {
        int index = Main.rng.RandiRange(0, data.Length - 1);
        float randRotation = Main.rng.RandfRange(0, (float)Math.Tau);

        //GD.Print("UV: " + UV[0]);
        //GD.Print("Data: " + data[0].Polygon[0]);
        FallSpeed += Main.rng.RandfRange(-20, 20);
        
        UV = data[index].Polygon;
        Polygon = data[index].Polygon;
        Vector2[] copy = UV;
        for (int i = 0; i < copy.Length; i++)
        {
            //copy[i] = copy[i].Rotated(randRotation);
            copy[i] = copy[i] + Position;
            copy[i] = copy[i] + new Vector2(20, 12);
        }
        UV = copy;
    }

    public override void _Process(double delta)
    {
        if (GlobalTimeState.Instance.TimeStopDuration <= 0)
        {
            Vector2 vel = new Vector2(0, FallSpeed) * (float)delta;
            Position += vel;
            Vector2[] copy = UV;
            for (int i = 0; i < copy.Length; i++)
            {
                copy[i] = copy[i] + vel;
            }
            UV = copy;
        }
    }

    public void OnOffscreen()
    {
        QueueFree();
    }
}
