using Godot;
using System;

public partial class MusicInfo : RichTextLabel
{
    [Export]
    Timer showTimer;
    [Export]
    Timer hideTimer;
    public bool hiding = true;

    public override void _EnterTree()
    {
        BackgroundMusicManager.Instance.OnBackgroundMusicStart += OnMusicStarted;
        //VisibleRatio = 0;
    }

    public override void _Process(double delta)
    {
        if (!hiding)
        {
            if (VisibleRatio < 1)
            {
                VisibleRatio += (float)(delta * 0.6);
            } else
            {
                VisibleRatio = 1;
            }
        } else
        {
            if (VisibleRatio > 0)
            {
                VisibleRatio -= (float)(delta * 0.6);
            } else
            {
                VisibleRatio = 0;
            }
        }
    }

    public void OnMusicStarted(MusicTheme theme)
    {
        Text = $"♪[b]{theme.SongName}[/b] by [b]{theme.AuthorName}[/b]♪";
        showTimer.Start(2);
    }

    public void OnShowTimerUp()
    {
        hiding = false;
        hideTimer.Start(8);
    }

    public void OnHideTimerUp()
    {
        hiding = true;
    }
}
