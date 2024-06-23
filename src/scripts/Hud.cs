using Godot;

namespace DotnetGDGame.scripts;

public partial class Hud : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    // private void ShowMessage(string text)
    // {
    //     var message = GetNode<Label>("Message");
    //     message.Text = text;
    //     message.Show();
    //
    //     // GetNode<Timer>("MessageTimer").Start();
    // }
    //
    // private async void ShowGameOver()
    // {
    //     ShowMessage("Game Over");
    //
    //     // var messageTimer = GetNode<Timer>("MessageTimer");
    //     // await ToSignal(messageTimer, Timer.SignalName.Timeout);
    //     //
    //     // var message = GetNode<Label>("Message");
    //     // message.Text = "Dodge the Creeps!";
    //     // message.Show();
    //
    //     await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
    //     GetNode<Button>("StartButton").Show();
    // }
    //
    private void OnStartButtonPressed()
    {
        GetNode<Button>("StartButton").Hide();
        EmitSignal(SignalName.StartGame);
    }
}