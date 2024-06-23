using Godot;

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

    private bool _isDashing = false;
    private bool _isDead = false;
    private float _dashTime = 0;

    private Vector2 _screenSize;
    private Vector2 _dashDirection = Vector2.Zero;
    private Vector2 _lastDirection = Vector2.Right;
    private AnimatedSprite2D _animatedSprite2D;
    private CollisionShape2D _collisionShape2D;

    public override void _Ready()
    {
        Hide();
        _screenSize = GetViewportRect().Size;
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public override void _Process(double delta)
    {
        if (_isDead) return;

        if (_isDashing)
        {
            UpdateDash(delta);
            return;
        }

        Vector2 velocity = GetInputVelocity();

        if (velocity != Vector2.Zero)
        {
            _lastDirection = velocity;

            if (Input.IsActionJustPressed("dash"))
            {
                StartDash(velocity);
                return;
            }

            velocity = velocity.Normalized() * Speed;
        }

        UpdatePositionAndAnimation(velocity, delta);
    }

    private void UpdateDash(double delta)
    {
        _dashTime -= (float)delta;

        if (_dashTime <= 0)
        {
            _isDashing = false;
            _animatedSprite2D.Stop();
        }
        else
        {
            MovePlayer(_dashDirection * DashSpeed, delta);
        }
    }

    private void UpdatePositionAndAnimation(Vector2 velocity, double delta)
    {
        UpdateAnimation(velocity);
        MovePlayer(velocity, delta);
    }

    private void MovePlayer(Vector2 velocity, double delta)
    {
        Position += velocity * (float)delta;
        Position = new Vector2(
            Mathf.Clamp(Position.X, 0, _screenSize.X),
            Mathf.Clamp(Position.Y, 0, _screenSize.Y)
        );
    }

    private static Vector2 GetInputVelocity()
    {
        Vector2 velocity = Vector2.Zero;

        if (Input.IsActionPressed("move_right")) velocity.X += 1;
        if (Input.IsActionPressed("move_left")) velocity.X -= 1;
        if (Input.IsActionPressed("move_down")) velocity.Y += 1;
        if (Input.IsActionPressed("move_up")) velocity.Y -= 1;

        return velocity;
    }

    private void UpdateAnimation(Vector2 velocity)
    {
        if (_isDashing)
        {
            PlayAnimation("dash");
            return;
        }

        if (velocity == Vector2.Zero)
        {
            UpdateIdleAnimation();
        }
        else
        {
            UpdateMovementAnimation(velocity);
        }
    }

    private void UpdateIdleAnimation()
    {
        if (_lastDirection.X != 0)
        {
            PlayAnimation("idle_side");
            _animatedSprite2D.FlipH = _lastDirection.X < 0;
        }
        else if (_lastDirection.Y > 0)
        {
            PlayAnimation("idle_down");
        }
        else if (_lastDirection.Y < 0)
        {
            PlayAnimation("idle_up");
        }
    }

    private void UpdateMovementAnimation(Vector2 velocity)
    {
        if (velocity.X != 0)
        {
            PlayAnimation("walk");
            _animatedSprite2D.FlipH = velocity.X < 0;
            _animatedSprite2D.FlipV = false;
        }
        else if (velocity.Y > 0)
        {
            PlayAnimation("down");
            _animatedSprite2D.FlipH = false;
            _animatedSprite2D.FlipV = false;
        }
        else if (velocity.Y < 0)
        {
            PlayAnimation("up");
            _animatedSprite2D.FlipH = false;
            _animatedSprite2D.FlipV = false;
        }
    }

    private void PlayAnimation(string animation)
    {
        _animatedSprite2D.Animation = animation;
        _animatedSprite2D.Play();
    }

    private void StartDash(Vector2 direction)
    {
        if (direction == Vector2.Zero) return;

        _isDashing = true;
        _dashTime = DashDuration;
        _dashDirection = direction.Normalized();
        PlayAnimation("dash");
    }

    private void OnBodyEntered(Node2D body)
    {
        if (!_isDead && body is Mob)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        _collisionShape2D.CallDeferred("set_disabled", true);
        _animatedSprite2D.Animation = "death";
        _animatedSprite2D.Play();
        EmitSignal(SignalName.Hit);
    }

    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        _collisionShape2D.Disabled = false;
    }
}
