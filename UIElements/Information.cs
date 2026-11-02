using Godot;
using System;

public partial class Information : Control
{
    [Export]
    MusicInfo Music;

    public void SetBorders(Vector2 topLeft, Vector2 bottomRight)
    {
        //SetSize(bottomRight - topLeft);
        //SetPosition(topLeft);
    }
}
