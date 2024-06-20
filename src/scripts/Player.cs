using Godot;
using System;

public partial class Player : Area2D
{
    [Export]
    public int Speed { get; set; } = 200;

    [Export]
    public int DashSpeed { get; set; } = 600;

    [Export]
    public float DashDuration { get; set; } = 0.2f;

    [Signal]
    public delegate void HitEventHandler();

    private Vector2 _screenSize;
    private AnimatedSprite2D _animatedSprite2D;
    private CollisionShape2D _collisionShape2D;
    private bool _isDashing = false;
    private float _dashTime = 0;
    private Vector2 _dashDirection = Vector2.Zero;
    private static Vector2 InputVelocity
    {
        get
        {
            Vector2 velocity = Vector2.Zero;

            if (Input.IsActionPressed("move_right"))
            {
                velocity.X += 1;
            }

            if (Input.IsActionPressed("move_left"))
            {
                velocity.X -= 1;
            }

            if (Input.IsActionPressed("move_down"))
            {
                velocity.Y += 1;
            }

            if (Input.IsActionPressed("move_up"))
            {
                velocity.Y -= 1;
            }

            return velocity;
        }
    }

    public override void _Ready()
    {
        Hide();
        _screenSize = GetViewportRect().Size;
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public override void _Process(double delta)
    {
        if (_isDashing)
        {
            _dashTime -= (float)delta;
            if (_dashTime <= 0)
            {
                _isDashing = false;
                _animatedSprite2D.Stop();
            }
            else
            {
                Position += _dashDirection * DashSpeed * (float)delta;
                Position = new Vector2(
                    x: Mathf.Clamp(Position.X, 0, _screenSize.X),
                    y: Mathf.Clamp(Position.Y, 0, _screenSize.Y)
                );
                return;
            }
        }

        Vector2 velocity = InputVelocity;
        UpdateAnimation(velocity);

        if (Input.IsActionJustPressed("dash"))
        {
            StartDash(velocity);
        }
        else
        {
            if (velocity != Vector2.Zero)
            {
                velocity = velocity.Normalized() * Speed;
            }

            Position += velocity * (float)delta;
            Position = new Vector2(
                x: Mathf.Clamp(Position.X, 0, _screenSize.X),
                y: Mathf.Clamp(Position.Y, 0, _screenSize.Y)
            );
        }
    }

    private void UpdateAnimation(Vector2 velocity)
    {
        if (_isDashing)
        {
            _animatedSprite2D.Animation = "dash";
            _animatedSprite2D.Play();
            return;
        }

        if (velocity == Vector2.Zero)
        {
            _animatedSprite2D.Stop();
            return;
        }

        _animatedSprite2D.Play();

        if (velocity.X != 0)
        {
            _animatedSprite2D.Animation = "walk";
            _animatedSprite2D.FlipV = false;
            _animatedSprite2D.FlipH = velocity.X < 0;
        }
        else if (velocity.Y > 0)
        {
            _animatedSprite2D.Animation = "down";
        }
        else if (velocity.Y < 0)
        {
            _animatedSprite2D.Animation = "up";
        }
    }

    private void StartDash(Vector2 direction)
    {
        if (direction == Vector2.Zero)
        {
            return;
        }

        _isDashing = true;
        _dashTime = DashDuration;
        _dashDirection = direction.Normalized();
        _animatedSprite2D.Animation = "dash";
        _animatedSprite2D.Play();
    }

    private void OnBodyEntered()
    {
        Hide();
        EmitSignal(SignalName.Hit);
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }

    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }
}
