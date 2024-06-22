using Godot;
using System;

public partial class Mob : RigidBody2D
{
    private AnimatedSprite2D _animatedSprite2D;
    private string[] _mobTypes;

    [Export]
    public int Speed { get; set; } = 200;

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _mobTypes = _animatedSprite2D.SpriteFrames.GetAnimationNames();

        PlayRandomAnimation();
        SetInitialProperties();
    }

    private void PlayRandomAnimation()
    {
        var randomIndex = GD.Randi() % _mobTypes.Length;
        _animatedSprite2D.Play(_mobTypes[randomIndex]);
    }

    private void SetInitialProperties()
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();
        Position = spawnPosition;

        Vector2 direction = GetDirectionToOppositeSide(spawnPosition);
        LinearVelocity = direction * Speed;

        _animatedSprite2D.FlipH = direction.X < 0;

        AngularVelocity = 0;
        Rotation = 0;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Rect2 viewportRect = GetViewportRect();

        uint side = GD.Randi() % 4;

        switch (side)
        {
            case 0:
                return new Vector2(GD.Randf() * viewportRect.Size.X, 0);
            case 1:
                return new Vector2(GD.Randf() * viewportRect.Size.X, viewportRect.Size.Y);
            case 2:
                return new Vector2(0, GD.Randf() * viewportRect.Size.Y);
            case 3:
                return new Vector2(viewportRect.Size.X, GD.Randf() * viewportRect.Size.Y);
            default:
                return Vector2.Zero;
        }
    }

    private Vector2 GetDirectionToOppositeSide(Vector2 spawnPosition)
    {
        Rect2 viewportRect = GetViewportRect();

        if (spawnPosition.Y == 0)
        {
            return new Vector2(GD.Randf() * 2 - 1, 1).Normalized();
        }
        else if (spawnPosition.Y == viewportRect.Size.Y)
        {
            return new Vector2(GD.Randf() * 2 - 1, -1).Normalized();
        }
        else if (spawnPosition.X == 0)
        {
            return new Vector2(1, GD.Randf() * 2 - 1).Normalized();
        }
        else if (spawnPosition.X == viewportRect.Size.X)
        {
            return new Vector2(-1, GD.Randf() * 2 - 1).Normalized();
        }

        return Vector2.Zero;
    }

    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }
}
