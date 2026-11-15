using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Planet : Control
{
    [Export]
    public ColorRect Land;
    [Export]
    public ColorRect Skies;
    [Export]
    public AnimatedSprite2D Shine;

    [Export]
    public int PlanetSize = 100;
    [Export]
    public float LandRotationSpeed = 0.3f;
    [Export]
    public float SkiesRotationSpeed = 0.5f;

    [Export]
    public double MaxTime = 30;
    [Export]
    public double Time = 0;
    [Export]
    public float TimeMultiplier = 1;
    [Export]
    public Vector2 PlanetPos = Vector2.Zero;

    public override void _EnterTree()
    {
        MaxTime += Main.rng.RandfRange(-10, 60);

        (Land.Material as ShaderMaterial).SetShaderParameter("seed", Main.rng.RandfRange(0, 1000));
        (Land.Material as ShaderMaterial).SetShaderParameter("pixels", PlanetSize);

        (Skies.Material as ShaderMaterial).SetShaderParameter("seed", Main.rng.RandfRange(0, 1000));
        (Skies.Material as ShaderMaterial).SetShaderParameter("pixels", PlanetSize);
        
        List<Color> landColors = new();
        List<Color> skyColors = new();

        List<Color> baseColors = GetRandomPalette(
            3, Main.rng.RandfRange(0.7f, 1f), Main.rng.RandfRange(0.45f, 0.55f)
        );

        for (int i = 0; i < 4; i++)
        {
            landColors.Add(baseColors[0].Darkened(i / 4f));
        }
        for (int i = 0; i < 2; i++)
        {
            landColors.Add(baseColors[1].Darkened(i / 2f));
        }
        for (int i = 0; i < 4; i++)
        {
            skyColors.Add(baseColors[2].Darkened(i / 4f));
        }

        (Land.Material as ShaderMaterial).SetShaderParameter("river_cutoff", Main.rng.RandfRange(0.15f, 0.45f));
        (Land.Material as ShaderMaterial).SetShaderParameter("size", Main.rng.RandiRange(3, 8));
        (Land.Material as ShaderMaterial).SetShaderParameter("OCTAVES", Main.rng.RandiRange(2, 6));
        
        (Skies.Material as ShaderMaterial).SetShaderParameter("cloud_cover", Main.rng.RandfRange(0.28f, 0.8f));
        (Skies.Material as ShaderMaterial).SetShaderParameter("stretch", Main.rng.RandfRange(1f, 3f));
        (Skies.Material as ShaderMaterial).SetShaderParameter("cloud_curve", Main.rng.RandfRange(1.3f, 2f));
        (Skies.Material as ShaderMaterial).SetShaderParameter("size", Main.rng.RandfRange(2, 16));
        (Skies.Material as ShaderMaterial).SetShaderParameter("OCTAVES", Main.rng.RandiRange(2, 8));

        if (landColors.Count > 0)
        {
            (Land.Material as ShaderMaterial).SetShaderParameter("colors", landColors.ToArray());
        }

        if (skyColors.Count > 0)
        {
            (Skies.Material as ShaderMaterial).SetShaderParameter("colors", landColors.ToArray());
        }

        float newRotation = Main.rng.RandfRange(-(float)Math.PI / 2f, (float)Math.PI / 2f);
        (Land.Material as ShaderMaterial).SetShaderParameter(
            "rotation", newRotation
        );
        (Skies.Material as ShaderMaterial).SetShaderParameter(
            "rotation", newRotation
        );

        Land.Size = Vector2.One * PlanetSize;
        Land.Position = PlanetPos - (Land.Size / 2f);
        Skies.Size = Vector2.One * PlanetSize;
        Skies.Position = PlanetPos - (Skies.Size / 2f);

        Shine.Position = PlanetPos;
        Shine.Scale = Vector2.One * (PlanetSize / 100f);
    }

    public override void _Process(double delta)
    {
        if (GlobalTimeState.Instance.TimeStopDuration <= 0)
        {
            Time += delta;
            float decrease = (float)delta / 1.5f;

            if (Shine.Scale.X > 0 && Shine.Scale.Y > 0)
            {
                Shine.Scale -= Vector2.One * decrease;
            }
            if (Shine.Modulate.A > 0)
            {
                Shine.Modulate = new(Shine.Modulate.R, Shine.Modulate.G, Shine.Modulate.B, Shine.Modulate.A - decrease);
            }

            if (Time >= MaxTime)
            {
                Modulate = new(Modulate.R, Modulate.G, Modulate.B, Modulate.A - (float)delta);
                if (Modulate.A <= 0)
                {
                    QueueFree();
                }
            }
        }

        (Land.Material as ShaderMaterial).SetShaderParameter("time", Time * TimeMultiplier * LandRotationSpeed);
        (Land.Material as ShaderMaterial).SetShaderParameter("alpha", Modulate.A);
        (Skies.Material as ShaderMaterial).SetShaderParameter("time", Time * TimeMultiplier * SkiesRotationSpeed);
        (Skies.Material as ShaderMaterial).SetShaderParameter("alpha", Modulate.A);
    }

    // Even now I am not entirely clear about how this works
    public List<Color> GetRandomPalette(int num = 1, float hueDiff = 0.9f, float saturation = 0.5f) 
    {
        List<Color> newList = new();

        Vector3 neutralBase = new(0.5f, 0.5f, 0.5f);
        Vector3 saturationOffset = new Vector3(0.5f, 0.5f, 0.5f) * saturation;
        Vector3 hueBase = new Vector3(
            Main.rng.RandfRange(0.5f, 1.5f), Main.rng.RandfRange(0.5f, 1.5f), Main.rng.RandfRange(0.5f, 1.5f)
        ) * hueDiff;
        Vector3 randomness = new Vector3(
            Main.rng.RandfRange(0.0f, 1.0f), Main.rng.RandfRange(0.0f, 1.0f), Main.rng.RandfRange(0.0f, 1.0f)
        ) * Main.rng.RandfRange(1.0f, 3.0f);

        for (int i = 0; i < num; i++)
        {
            float div = i / (float)num;
            Color color = new Color(
                (neutralBase.X + saturationOffset.X * (float)Math.Cos(Math.Tau * (hueBase.X * div + randomness.X))), 
                (neutralBase.Y + saturationOffset.Y * (float)Math.Cos(Math.Tau * (hueBase.Y * div + randomness.Y))), 
                (neutralBase.Z + saturationOffset.Z * (float)Math.Cos(Math.Tau * (hueBase.Z * div + randomness.Z)))
            );
            
            newList.Add(color);
        }

        return newList;
    }
}
