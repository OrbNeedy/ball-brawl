using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node2D
{
    [Signal]
    public delegate void TeamWonEventHandler();
    [Export]
    public Marker2D WorldPosition;
    [Export]
    public Marker2D WorldSize;
    [Export]
    public PhysicsMaterial BoundaryMaterial = new();
    [Export]
    public PhysicsMaterial TopBoundaryMaterial = new();
    [Export]
    public PackedScene[] Backgrounds = new PackedScene[0];

    [Export]
    Node2D ProjectileContainer;
    [Export]
    Node2D BallContainer;
    [Export]
    HealthUi HealthUI;
    [Export]
    VictoryScreen VictoryScreen;
    [Export]
    Information InfoUI;
    [Export]
    SubViewport BallsViewport;
    [Export]
    SubViewport ProjectilesViewport;
    [Export]
    Control BackgroundContainer;

    [Export]
    public MusicTheme ThemeOverride;
    [Export]
    public float TeamSetupRotation = 0;
    List<Ball> Entrances = new();
    int CurrentEntrance = 0;

    [Export]
    public Godot.Collections.Array<TeamData> TeamsInvolved = new();
    [Export]
    public Godot.Collections.Array<PackedScene> IndependentBalls;
    [Export]
    public SubViewportContainer viewport;

    public static RandomNumberGenerator rng = new RandomNumberGenerator();

    public Dictionary<string, int> RemainingTeams = new();
    public Dictionary<string, List<Ball>> SelectedTeams = new();

    public override void _EnterTree()
    {
        rng.Randomize();

        SetBoundaries();
    }

    public override void _Ready()
    {
        if (Backgrounds.Length <= 0) return;

        int selected = rng.RandiRange(0, Backgrounds.Length - 1);
        Background background = Backgrounds[selected].Instantiate<Background>();
        background.BallsViewport = BallsViewport;
        background.ProjectilesViewport = ProjectilesViewport;

        BackgroundContainer.AddChild(background);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustReleased("Screenshot"))
        {
            ViewportTexture texture = GetTree().Root.GetViewport().GetTexture();
            double time = Time.GetUnixTimeFromSystem();
            texture.GetImage().SavePng($"C:/Users/usuario/Downloads/BallBrawlScreenshot_{time}.png");
        }

        if (Input.IsActionJustReleased("Accept"))
        {
            SetupBalls();
        }
    }

    public void SetupBalls()
    {
        int teamCount = TeamsInvolved.Count + IndependentBalls.Count;
        int count = 0;
        
        float rotationalAmount = (float)Math.Tau / (float)teamCount;

        bool outline = TeamsInvolved.Count > 1 || teamCount >= 3;

        Vector2 totalSize = (WorldSize.Position - WorldPosition.Position);
        Vector2 center = (totalSize * 0.5f);
        Vector2 initialDirection = new Vector2(0.4f, 0).Rotated(float.DegreesToRadians(TeamSetupRotation));
        foreach (TeamData data in TeamsInvolved)
        {
            Vector2 teamPosition = center + (initialDirection.Rotated(count * rotationalAmount) * totalSize);
            SelectedTeams.Add(data.TeamName, new List<Ball>());
            foreach (PackedScene scene in data.Balls)
            {
                Vector2 offset = new Vector2(rng.RandfRange(0, data.SpawnDistance), 0).
                    Rotated(rng.RandfRange(0, (float)Math.Tau));

                Ball ball = scene.Instantiate<Ball>();
                ball.TeamName = data.TeamName;
                ball.Color = data.TeamColor;
                ball.Translate(teamPosition + offset);
                ball.TeamOutline = outline;
                SelectedTeams[data.TeamName].Add((Ball)ball.Duplicate());

                HealthUI.AddBallPreFight(ball);
                if (ball.Entrance != null)
                {
                    Entrances.Add(ball);
                }

                AddBall(ball);
            }
            count++;
        }

        foreach (PackedScene scene in IndependentBalls)
        {
            Vector2 teamPosition = center + (initialDirection.Rotated(count * rotationalAmount) * totalSize);

            Ball ball = scene.Instantiate<Ball>();
            ball.Position = teamPosition;
            ball.TeamOutline = outline;
            ball.TeamName = ball.BallName;

            SelectedTeams[ball.BallName] = new() { (Ball)ball.Duplicate() };

            HealthUI.AddBallPreFight(ball);
            if (ball.Entrance != null)
            {
                Entrances.Add(ball);
            }

            AddBall(ball);
            count++;
        }

        String randTeam = (String)RandFromList(SelectedTeams.Keys.ToArray());
        Ball randBall = (Ball)RandFromList(SelectedTeams[randTeam].ToArray());
        
        BackgroundMusicManager.Instance.CurrentTheme = randBall.BackgroundTheme;
        if (ThemeOverride != null)
        {
            BackgroundMusicManager.Instance.CurrentTheme = ThemeOverride;
        }
        
        if (Entrances.Count > 0)
        {
            GetTree().Paused = true;

            Entrances.OrderBy((b) =>
            {
                return b.Entrance.Priority;
            });

            foreach (Ball ball in Entrances)
            {
                ball.Entrance.EntranceFinished += OnEntranceFinished;
                ball.Entrance.InitialSetup(ball);
            }
        } else
        {
            StartMatch();
        }
    }

    public void OnEntranceFinished()
    {
        CurrentEntrance++;
        if (CurrentEntrance >= Entrances.Count)
        {
            StartMatch();
        } else
        {
            Entrances[CurrentEntrance].Entrance.InitialSetup(Entrances[CurrentEntrance]);
        }
    }

    public void StartMatch()
    {
        GetTree().Paused = false;
        HealthUI.SetBars();

        BackgroundMusicManager.Instance.PlayTheme();
    }

    public void AddBall(Ball ball)
    {
        ball.SpawnProjectile += AddProjectile;
        ball.SpawnBall += AddBall;
        ball.BallDead += OnBallDead;
        ball.SpawnParticles += AddParticle;
        ball.SpawnNode += AddNode;
        TeamWon += ball.Won;

        BallContainer.AddChild(ball);

        if (!RemainingTeams.ContainsKey(ball.TeamName))
        {
            RemainingTeams[ball.TeamName] = 0;
        }

        RemainingTeams[ball.TeamName]++;
    }

    public void AddProjectile(Projectile projectile)
    {
        projectile.SpawnProjectile += AddProjectile;
        projectile.SpawnBall += AddBall;
        projectile.SpawnParticles += AddParticle;

        ProjectileContainer.AddChild(projectile);
    }

    public void AddParticle(SelfDeletingParticles particle)
    {
        ProjectileContainer.AddChild(particle);
    }

    // Any node
    public void AddNode(Node node)
    {
        BallContainer.AddChild(node);
    }

    public void OnBallDead(Ball ball)
    {
        TeamWon -= ball.Won;
        if (RemainingTeams.ContainsKey(ball.TeamName))
        {
            RemainingTeams[ball.TeamName]--;
            if (RemainingTeams[ball.TeamName] <= 0)
            {
                RemainingTeams.Remove(ball.TeamName);
            }
        }

        if (RemainingTeams.Count == 1)
        {
            string winnerTeam = RemainingTeams.Keys.ToList()[0];
            EmitSignal(SignalName.TeamWon, winnerTeam);
            GetTree().Paused = true;

            Timer victoryTimer = new Timer();
            victoryTimer.OneShot = true;
            victoryTimer.Timeout += () =>
            {
                VictoryScreen.Activate(SelectedTeams[winnerTeam], winnerTeam);
            };

            AddChild(victoryTimer);
            victoryTimer.Start(2);
        }

        if (RemainingTeams.Count == 0)
        {
            // Draw, somehow
            GetTree().Paused = true;
        }
    }

    public void SetBoundaries()
    {
        // Left bound
        StaticBody2D left = new StaticBody2D
        {
            PhysicsMaterialOverride = BoundaryMaterial, 
            //Position = WorldPosition.Position
        };
        WorldBoundaryShape2D leftShape = new WorldBoundaryShape2D
        {
            Normal = new Vector2(1, 0)
        };
        CollisionShape2D leftCollision = new CollisionShape2D
        {
            Shape = leftShape
        };
        left.AddChild(leftCollision);
        left.AddToGroup("Boundary");
        BallContainer.AddChild(left);
        
        // Right bound
        StaticBody2D right = new StaticBody2D
        {
            PhysicsMaterialOverride = BoundaryMaterial, 
            Position = WorldSize.Position - WorldPosition.Position
        };
        WorldBoundaryShape2D rightShape = new WorldBoundaryShape2D
        {
            Normal = new Vector2(-1, 0)
        };
        CollisionShape2D rightCollision = new CollisionShape2D
        {
            Shape = rightShape
        };
        right.AddChild(rightCollision);
        right.AddToGroup("Boundary");
        BallContainer.AddChild(right);
        
        // Top bound
        StaticBody2D top = new StaticBody2D
        {
            PhysicsMaterialOverride = TopBoundaryMaterial, 
            //Position = WorldPosition.Position
        };
        WorldBoundaryShape2D topShape = new WorldBoundaryShape2D
        {
            Normal = new Vector2(0, 1)
        };
        CollisionShape2D topCollision = new CollisionShape2D
        {
            Shape = topShape
        };
        top.AddChild(topCollision);
        top.AddToGroup("Boundary");
        BallContainer.AddChild(top);
        
        // Bottom bound
        StaticBody2D bottom = new StaticBody2D
        {
            PhysicsMaterialOverride = BoundaryMaterial, 
            Position = WorldSize.Position - WorldPosition.Position
        };
        WorldBoundaryShape2D bottomShape = new WorldBoundaryShape2D
        {
            Normal = new Vector2(0, -1)
        };
        CollisionShape2D bottomCollision = new CollisionShape2D
        {
            Shape = bottomShape
        };
        bottom.AddChild(bottomCollision);
        bottom.AddToGroup("Boundary");
        BallContainer.AddChild(bottom);

        viewport.Position = WorldPosition.Position;
        viewport.Size = WorldSize.Position - WorldPosition.Position;

        //BallContainer.Position = ProjectileContainer.Position = -WorldPosition.Position;
        InfoUI.SetBorders(WorldPosition.Position, WorldSize.Position);
        HealthUI.BattleViewportPosition = WorldPosition.Position;
    }

    public static object RandFromList(Array array)
    {
        if (array.Length <= 0) return null;

        int index = rng.RandiRange(0, array.Length - 1);

        return array.GetValue(index);
    }
}
