using Godot;
using System;

public partial class CircularProjectile : Projectile
{
    [Export]
    public string Version = "default";
    [Export]
    public int Frame = 0;

    public override void OnReady()
    {
        body.Animation = Version;
        body.Frame = Frame;
    }
}
