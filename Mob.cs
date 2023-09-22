using Godot;
using System;

public partial class Mob : RigidBody2D {

	private AnimatedSprite2D _animatedSprite;
	private VisibleOnScreenNotifier2D _visibleOnScreen;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_visibleOnScreen = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		Start();
	}

	private void Start() {
		_visibleOnScreen.ScreenExited += OnVisibleOnScreenNotifierExited;
		string[] mobTypes = _animatedSprite.SpriteFrames.GetAnimationNames();
		_animatedSprite.Play(mobTypes[GD.Randi() % mobTypes.Length]);
	}

	private void OnVisibleOnScreenNotifierExited() {
		QueueFree();
	}
}
