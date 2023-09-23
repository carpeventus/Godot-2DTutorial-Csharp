using Godot;
using System;
using System.Diagnostics;

public partial class Main : Node {

	[Export] public PackedScene MobScene;
	private int _score;
	private Player _player;
	private Timer _startTimer;
	private Timer _mobTimer;
	private Timer _scoreTimer;
	private HUD _hud;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		_player = GetNode<Player>("Player");
		_startTimer = GetNode<Timer>("StartTimer");
		_mobTimer = GetNode<Timer>("MobTimer");
		_scoreTimer = GetNode<Timer>("ScoreTimer");
		_hud = GetNode<HUD>("HUD");
		
		_player.Hit += GameOver;
		_startTimer.Timeout += OnStartTimerTimeout;
		_mobTimer.Timeout += OnMobTimerTimeout;
		_scoreTimer.Timeout += OnScoreTimerTimeout;
		_hud.StartGame += NewGame;
	}
	

	private void GameOver() {
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("GameOver").Play();
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		_hud.ShowGameOver();
		
	}

	private void NewGame() {
		GetTree().CallGroup("Mobs", Node.MethodName.QueueFree);
		_score = 0;
		var startPosition = GetNode<Marker2D>("StartPosition").Position;
		_player.Start(startPosition);
		GetNode<AudioStreamPlayer>("Music").Play();
		_hud.UpdateScore(_score);
		_hud.ShowMessage("Get Ready!");
		_hud.GetNode<Label>("ScoreLabel").Show();
		_startTimer.Start();
	}

	/**
	 * 游戏开始启动敌人和分数计时器
	 * NewGame -> OnStartTimerTimeout
	 */
	private void OnStartTimerTimeout() {
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}



	private void OnScoreTimerTimeout() {
		_score++;
		_hud.UpdateScore(_score);
	}
	
	private void OnMobTimerTimeout()
	{
		Mob mob = MobScene.Instantiate<Mob>();
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();
		// 随着ProgressRatio不同mobSpawnLocation.Rotation也会随着变化，设想有一个实体使用了Path2D，它始终面朝路径方向，因此不仅Position、Rotation也会跟着变化
		// 加上pi/2，顺时针旋转90度。是要将敌人朝向旋转到面向屏幕内部
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Position = mobSpawnLocation.Position;
		mob.Rotation = direction;

		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		// 将前进速度设置为和的敌人面朝方向一致
		mob.LinearVelocity = velocity.Rotated(direction);
		AddChild(mob);

	}
}
