using Godot;
using System;

public partial class MirrorReflections : Background
{
    [Export]
    public PackedScene MirrorScene;
    [Export]
    public double MaxMirrorFallCooldown = 3;
    public double MirrorFallCooldown = 0;

    public override void _Process(double delta)
    {
        if (MirrorFallCooldown <= 0)
        {
            int amount = Main.rng.RandiRange(1, 2);
            for (int i = 0; i < amount; i++)
            {
                MirrorShard shard = MirrorScene.Instantiate<MirrorShard>();

                shard.Position = new Vector2(Main.rng.RandfRange(0, Size.X), -128);
                // shard.Rotation = Main.rng.RandfRange(0, (float)Math.Tau);
                shard.Texture = BallsViewport.GetTexture();

                AddChild(shard);
            }
            MirrorFallCooldown = Main.rng.RandfRange(0.1f, (float)MaxMirrorFallCooldown);
        } else
        {
            if (GlobalTimeState.Instance.TimeStopDuration <= 0)
            {
                MirrorFallCooldown -= delta;
            }
        }
    }
}
