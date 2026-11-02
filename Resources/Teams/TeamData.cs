using Godot;
using Godot.Collections;
using System;

public partial class TeamData : Resource
{
    [Export]
    public String TeamName;
    [Export]
    public Color TeamColor = new Color();
    [Export]
    public float SpawnDistance;
    [Export]
    public Array<PackedScene> Balls = new();
    [Export]
    public EntranceData GroupEntrance;
}
