using Godot;
using System;

public partial class Tasouken : Weapon
{
    [Export]
    public PackedScene cut;
    [Export]
    public double MaxCutCooldown = 4;
    public double CutCooldown = 0;
    [Export]
    public double MaxRandomCutCooldown = 3;
    public double RandomCutCooldown = 0;
    [Export]
    public int RandomCutCount = 1;

    public override void OnReady()
    {
        RandomCutCooldown = MaxRandomCutCooldown / 2f;
    }

    public override void OnPhysicsProcess(double delta)
    {
    }

    public override void OnProcess(double delta)
    {
        if (CutCooldown > 0)
        {
            CutCooldown -= delta;
        }

        if (RandomCutCooldown > 0)
        {
            RandomCutCooldown -= delta;
        } else
        {
            for (int i = 0; i < RandomCutCount; i++)
            {
                Cut newCut = cut.Instantiate<Cut>();
                newCut.TeamName = TeamName;
                newCut.Rotate(Main.rng.RandfRange(0, (float)Math.Tau));
                newCut.Position = Holder.Position + new Vector2(200, 0).Rotated(Main.rng.RandfRange(0, (float)Math.Tau));
                newCut.TimeLeft = 1.8;
                newCut.Delay = 0.7;
                newCut.Damage.BaseDamage = 6;
                newCut.LengthMultiplier = 10;

                Holder.EmitSignal(Ball.SignalName.SpawnProjectile, newCut);
            }

            RandomCutCooldown = MaxRandomCutCooldown;
        }
    }

    public override void OnBallHit(Ball ball)
    {
        if (TeamName != "" && ball.IsInGroup(TeamName)) return;

        if (CutCooldown <= 0)
        {
            Cut newCut = cut.Instantiate<Cut>();
            newCut.TeamName = TeamName;
            newCut.Rotate(Main.rng.RandfRange(0, (float)Math.Tau));
            newCut.Position = ball.Position;
            newCut.Damage.BaseDamage = 3;

            Holder.CallDeferred(MethodName.EmitSignal, Ball.SignalName.SpawnProjectile, newCut);
            // Holder.EmitSignal(Ball.SignalName.SpawnProjectile, newCut);

            CutCooldown = MaxCutCooldown;
        }
    }
}
