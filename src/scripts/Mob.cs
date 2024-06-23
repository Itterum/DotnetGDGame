using System;
using Godot;

namespace DotnetGDGame.scripts;

public partial class Mob : RigidBody2D
{
    private AnimatedSprite2D _animatedSprite2D;
    private string[] _mobTypes;

    [Export] private int Speed { get; set; } = 200;

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
        var spawnPosition = GetRandomSpawnPosition();
        Position = spawnPosition;

        var direction = GetDirectionToOppositeSide(spawnPosition);
        LinearVelocity = direction * Speed;

        _animatedSprite2D.FlipH = direction.X < 0;

        AngularVelocity = 0;
        Rotation = 0;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        var viewportRect = GetViewportRect();

        var side = GD.Randi() % 4;

        return side switch
        {
            0 => new Vector2(GD.Randf() * viewportRect.Size.X, 0),
            1 => new Vector2(GD.Randf() * viewportRect.Size.X, viewportRect.Size.Y),
            2 => new Vector2(0, GD.Randf() * viewportRect.Size.Y),
            3 => new Vector2(viewportRect.Size.X, GD.Randf() * viewportRect.Size.Y),
            _ => Vector2.Zero,
        };
    }

    private Vector2 GetDirectionToOppositeSide(Vector2 spawnPosition)
    {
        var viewportRect = GetViewportRect();

        if (spawnPosition.Y == 0)
        {
            return new Vector2(GD.Randf() * 2 - 1, 1).Normalized();
        }

        if (Math.Abs(spawnPosition.Y - viewportRect.Size.Y) < 7)
        {
            return new Vector2(GD.Randf() * 2 - 1, -1).Normalized();
        }

        if (spawnPosition.X == 0)
        {
            return new Vector2(1, GD.Randf() * 2 - 1).Normalized();
        }

        return Math.Abs(spawnPosition.X - viewportRect.Size.X) < 7 ? new Vector2(-1, GD.Randf() * 2 - 1).Normalized() : Vector2.Zero;
    }

    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }
}