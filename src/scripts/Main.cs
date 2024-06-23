using Godot;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    private int _score;

    public override void _Ready()
    {
        NewGame();
    }

    public override void _Process(double delta)
    {
    }

    public void GameOver()
    {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();
    }

    public void NewGame()
    {
        _score = 0;

        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();
    }

    private void OnScoreTimerTimeout()
    {
        _score++;
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    private void OnMobTimerTimeout()
    {
        MobScene = (PackedScene)GD.Load("res://scenes/Mob.tscn");

        if (MobScene == null)
        {
            GD.PrintErr("Failed to load MobScene.");
            return;
        }

        if (MobScene == null)
        {
            GD.PrintErr("MobScene is not loaded.");
            return;
        }

        Mob mob = MobScene.Instantiate<Mob>();

        if (mob == null)
        {
            GD.PrintErr("Failed to instantiate Mob.");
            return;
        }

        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");

        if (mobSpawnLocation == null)
        {
            GD.PrintErr("MobSpawnLocation node not found.");
            return;
        }

        mobSpawnLocation.ProgressRatio = GD.Randf();

        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        mob.Position = mobSpawnLocation.Position;

        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        AddChild(mob);
    }
}
