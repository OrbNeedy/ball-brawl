using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class VictoryScreen : Control
{
    [Export]
    public Marker2D Center;
    [Export]
    public SubViewport WinnerDimension;
    [Export]
    public Label Winner;
    [Export]
    public Label TeamName;
    [Export]
    public AudioStream VictorySound;

    public void Activate(List<Ball> winners, string teamName)
    {
        Visible = true;
        SoundEffectManager.Instance.PlaySound(VictorySound, 2);

        if (winners.Count > 1)
        {
            Winner.Text = "WINNERS";
        }
        TeamName.Text = teamName;
        
        if (winners.Any((b) => { return b.Boss; }))
        {
            Winner.Text = "FAILURE";
            TeamName.Text = "Boss victory";
        }
        
        float halfMeasure = 50f;

        float min = halfMeasure * (-winners.Count + 1);
        float max = halfMeasure * (winners.Count - 1);
        
        float offsetX = min;
        foreach (Ball ball in winners)
        {
            Vector2 offset = new Vector2(offsetX, 0);
            
            Ball duplicate = (Ball)ball.Duplicate();
            duplicate.Won();
            duplicate.TeamOutline = false;
            duplicate.Position = Center.Position + offset;
            offsetX += halfMeasure * 2;

            WinnerDimension.AddChild(duplicate);
        }
    }
}
