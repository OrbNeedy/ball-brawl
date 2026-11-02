using Godot;
using System;

public partial class MusicTheme : Resource
{
    [Export]
    public AudioStream Theme;
    [Export]
    public string SongName = "";
    [Export]
    public string AuthorName = "";
    [Export]
    public float StartingTime = 0.0f;
    [Export]
    public float EndingTime = 0.0f;
    [Export]
    public float Volume = 1.0f;
}
