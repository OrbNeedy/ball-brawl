using Godot;
using System;

public partial class MakotoEXEntrance : EntranceData
{
    public override void InitialSetup(Ball target)
    {
        target.Position -= new Vector2(0, 64);
        target.Modulate = Colors.Transparent;

        Tween tween = target.GetTree().CreateTween().SetTrans(Tween.TransitionType.Linear).
            SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenInterval(1);
        tween.TweenCallback(Callable.From(BackgroundMusicManager.Instance.PlayTheme));
        tween.SetParallel();
        tween.TweenProperty(target, "position", target.Position + new Vector2(0, 128), 3);
        tween.TweenProperty(target, "modulate", Colors.White, 4);
        tween.SetParallel(false);
        tween.TweenInterval(2);
        tween.TweenCallback(Callable.From(() => { EmitSignal(SignalName.EntranceFinished); }));
    }
}
