using Godot;

public partial class HUD : CanvasLayer {
    [Signal]
    public delegate void StartGameEventHandler();

    private Button _startButton; 
    private Timer _messageTimer; 
    public override void _Ready() {
        _startButton = GetNode<Button>("StartButton");
        _messageTimer = GetNode<Timer>("MessageTimer");
        _startButton.Pressed += OnStartButtonPressed;
        _messageTimer.Timeout += OnMessageTimeOut;
    }

    public void OnStartButtonPressed() {
        _startButton.Hide();
        EmitSignal(SignalName.StartGame);
    }
    
    public void OnMessageTimeOut() {
        GetNode<Label>("Message").Hide();
    }

    public void ShowMessage(string text) {
        var msg = GetNode<Label>("Message");
        msg.Text = text;
        msg.Show();
        GetNode<Timer>("MessageTimer").Start();
    }
    
    async public void ShowGameOver() {
        // 展示N秒的文案
        ShowMessage("Game Over");
        var messageTimer = GetNode<Timer>("MessageTimer");
        // 等待发送了Timeout信号后执行下面的代码
        await ToSignal(messageTimer, Timer.SignalName.Timeout);
        var msg = GetNode<Label>("Message");
        msg.Text = "Dodge the Creeps!";
        msg.Show();
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        GetNode<Button>("StartButton").Show();
    }

    public void UpdateScore(int score) {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
    }
}
