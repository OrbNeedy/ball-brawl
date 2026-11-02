using Godot;
using System;

public partial class BackgroundMusicManager : Node
{
    [Signal]
    public delegate void OnBackgroundMusicStartEventHandler(MusicTheme theme);
    public static BackgroundMusicManager Instance { get; private set; }

    public AudioStreamPlayer MusicPlayer = new();
    public MusicTheme CurrentTheme;

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

    public void PlayTheme(MusicTheme theme)
    {
        CurrentTheme = theme;
        
        MusicPlayer.Stream = CurrentTheme.Theme;
        MusicPlayer.VolumeLinear = CurrentTheme.Volume;
        MusicPlayer.Play(CurrentTheme.StartingTime);
        EmitSignal(SignalName.OnBackgroundMusicStart, CurrentTheme);
    }

    public void PlayTheme()
    {
        if (CurrentTheme != null)
        {
            if (MusicPlayer.Playing) return;
            
            MusicPlayer.Stream = CurrentTheme.Theme;
            MusicPlayer.VolumeLinear = CurrentTheme.Volume;
            MusicPlayer.Play(CurrentTheme.StartingTime);
            EmitSignal(SignalName.OnBackgroundMusicStart, CurrentTheme);
        }
    }
}
