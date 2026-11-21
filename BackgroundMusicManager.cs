using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BackgroundMusicManager : Node
{
    [Signal]
    public delegate void OnBackgroundMusicStartEventHandler(MusicTheme theme);
    public static BackgroundMusicManager Instance { get; private set; }

    public AudioStreamPlayer MusicPlayer = new();
    public List<MusicTheme> Themes = new();
    public int CurrentTheme;
    /// <summary>
    /// If true, every time a song is solicited or finishes, it will not be repeated, but a new song will start from the list.
    /// </summary>
    public bool Shuffle = false;

    public override void _EnterTree()
    {
        //GD.Print("Music player ready");
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
        AddChild(MusicPlayer);
    }

    public override void _Process(double delta)
    {
        MusicPlayer.StreamPaused = GlobalTimeState.Instance.TimeStopDuration > 0;
    }

    public void SetList(params MusicTheme[] themes)
    {
        Themes = themes.ToList();
        CurrentTheme = Main.rng.RandiRange(0, Themes.Count - 1);
    }

    public void PlayTheme(MusicTheme theme)
    {
        MusicPlayer.Stream = theme.Theme;
        MusicPlayer.VolumeLinear = theme.Volume;
        MusicPlayer.Play(theme.StartingTime);
        EmitSignal(SignalName.OnBackgroundMusicStart, theme);
    }

    public void PlayTheme()
    {
        if (Themes.Count > 0)
        {
            if (MusicPlayer.Playing) return;
            MusicTheme theme = Themes[CurrentTheme];
            
            MusicPlayer.Stream = theme.Theme;
            MusicPlayer.VolumeLinear = theme.Volume;
            MusicPlayer.Play(theme.StartingTime);
            EmitSignal(SignalName.OnBackgroundMusicStart, theme);

            if (Shuffle)
            {
                CurrentTheme = Main.rng.RandiRange(0, Themes.Count - 1);
            }
        }
    }
}
