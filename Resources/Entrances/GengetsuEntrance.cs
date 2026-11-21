using Godot;
using System;

public partial class GengetsuEntrance : EntranceData
{
    public override void InitialSetup(Ball target)
    {
        target.Position -= new Vector2(0, 256);
        target.Modulate = new Color(0.06f, 0f, 0.02f);
        
        Tween pulseTween = target.GetTree().CreateTween().SetTrans(Tween.TransitionType.Linear).
            SetPauseMode(Tween.TweenPauseMode.Process);
        for (int i = 0; i < 12; i++)
        {
            // Pulse
            pulseTween.TweenInterval(1.05);
            pulseTween.TweenCallback(Callable.From(() => { 
                target.EmitSignal(Ball.SignalName.BackgroundSignal, "pulse", new float[0]); 
            }));
        }


        Tween tween = target.GetTree().CreateTween().SetTrans(Tween.TransitionType.Linear).
            SetPauseMode(Tween.TweenPauseMode.Process);
        // Play Theme
        tween.TweenCallback(Callable.From(BackgroundMusicManager.Instance.PlayTheme));
        tween.TweenInterval(2);

        // Move
        tween.TweenProperty(target, "position", target.Position + new Vector2(0, 256), 10);

        // Brighten
        tween.TweenCallback(Callable.From(() => { 
            target.EmitSignal(Ball.SignalName.BackgroundSignal, "brighten", new float[0]); 
        }));
        tween.TweenProperty(target, "modulate", Colors.White, 2);

        tween.TweenInterval(2);
        tween.TweenCallback(Callable.From(() => { 
            EmitSignal(SignalName.EntranceFinished); 
        }));
    }
}
