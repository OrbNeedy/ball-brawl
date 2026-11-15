using Godot;
using System;
using System.Diagnostics;

public partial class DreamRealm : Background
{
    [Export]
    public PackedScene PlanetScene;

    [Export]
    public ColorRect Background;
    [Export]
    public Parallax2D Planets1;
    [Export]
    public Parallax2D Planets2;
    [Export]
    public Parallax2D Planets3;

    [Export]
    public Color InitialColor = Colors.White;
    [Export]
    public Color TargetColor = Colors.White;
    [Export]
    public double MaxColorTimer = 90;
    public double ColorTimer = -10;
    [Export]
    public Color FlashColor = Colors.White;
    [Export]
    public float MaxFlashDistance = 1000;
    [Export]
    public double MaxFlashTimer = 1.5;
    public double FlashTimer = 0;

    public override void _EnterTree()
    {
        for (int i = 0; i < 4; i++)
        {
            switch (Main.rng.RandiRange(0, 2))
            {
                default:
                case 0:
                    SpawnPlanet(Planets1, -1);
                    break;
                case 1:
                    SpawnPlanet(Planets2, 0);
                    break;
                case 2:
                    SpawnPlanet(Planets3, 1);
                    break;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (GlobalTimeState.Instance.TimeStopDuration > 0)
        {
            Planets1.Autoscroll = Vector2.Zero;
            Planets2.Autoscroll = Vector2.Zero;
            Planets3.Autoscroll = Vector2.Zero;
            return;
        } else
        {
            Planets1.Autoscroll = new Vector2(-35, 0);
            Planets2.Autoscroll = new Vector2(-20, 0);
            Planets3.Autoscroll = new Vector2(-5, 0);
            ColorTimer += delta;
        }

        if (Main.rng.Randf() <= 0.006f)
        {
            switch (Main.rng.RandiRange(0, 2))
            {
                default:
                case 0:
                    SpawnPlanet(Planets1, -1);
                    break;
                case 1:
                    SpawnPlanet(Planets2, 0);
                    break;
                case 2:
                    SpawnPlanet(Planets3, 1);
                    break;
            }
        }

        float colorProgress = float.Clamp((float)(ColorTimer / MaxColorTimer), 0, 1);
        Color bgColor = InitialColor.Lerp(TargetColor, colorProgress);

        float flashProgress = float.Clamp((float)Math.Log((FlashTimer / MaxFlashTimer * 9) + 1), 0, 1);
        Background.Color = bgColor.Lerp(FlashColor, flashProgress);

        if (FlashTimer > 0)
        {
            FlashTimer -= delta;
        }
    }

    public void SpawnPlanet(Parallax2D target, int dist)
    {
        Vector2 center = GetViewportRect().Size / 2f;
        float RepeatSize = target.RepeatSize.X; 
        Planet planet = PlanetScene.Instantiate<Planet>();
        planet.PlanetPos.X = Main.rng.RandfRange(-RepeatSize, RepeatSize) / 2f;
        planet.PlanetPos.Y = Main.rng.RandfRange(-80, (center.Y * 2) + 80);
        planet.PlanetSize += 50 * dist;
        planet.PlanetSize += Main.rng.RandiRange(-4, 4) * 10;

        target.AddChild(planet);

        float distance = center.DistanceTo(planet.Position - Planets1.ScreenOffset);
        float distanceMultiplier = ((MaxFlashDistance - distance) / MaxFlashDistance);
        double newFlash = MaxFlashTimer * distanceMultiplier;
        if (newFlash > FlashTimer)
        {
            FlashTimer = newFlash * 1.1f;
        }
    }
}
