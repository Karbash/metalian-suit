using Godot;

public partial class Game : Node
{
	public override void _Ready()
	{
		// Inicia pelo Title Screen
		GetTree().ChangeSceneToFile("res://ui/TitleScreen.tscn");
	}
}
