using Godot;

public partial class Player : Area2D {
    [Export] public int Speed { get; set; } = 400;
    public Vector2 ScreenSize;

    private Vector2 _direction;

    private AnimatedSprite2D _animatedSprite;

    [Signal]
    public delegate void HitEventHandler();

    public override void _Ready() {
        ScreenSize = GetViewportRect().Size;
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        BodyEntered += OnBodyEntered;
    }

    public void Start(Vector2 position) {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    private void OnBodyEntered(Node2D body) {
        Hide();
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        EmitSignal(SignalName.Hit);
    }

    public override void _Process(double delta) {
        var velocity = Vector2.Zero;
        _direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (!Mathf.IsZeroApprox(_direction.Length())) {
            velocity = Speed * _direction.Normalized() ;
        }
 
        _animatedSprite.FlipH = velocity.X < 0;
        _animatedSprite.FlipV = velocity.Y > 0;

        if (velocity.Y != 0) {
            _animatedSprite.Play("up");
        }
        else if (velocity.X != 0) {
            _animatedSprite.Play("walk");
        }
        else {
            _animatedSprite.Stop();
        }
        
        Position += velocity * (float)delta;
        Position = new Vector2(Mathf.Clamp(Position.X, 0, ScreenSize.X), Mathf.Clamp(Position.Y, 0, ScreenSize.Y));
    }
}
