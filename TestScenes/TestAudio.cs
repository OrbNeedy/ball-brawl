using Godot;
using System;

public partial class TestAudio : AudioStreamPlayer
{
    public override void _Process(double delta)
    {
        GetPlaybackPosition();
    }
}
