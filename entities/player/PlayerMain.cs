using Godot;

/// <summary>
/// Player principal - implementação específica
/// </summary>
public partial class PlayerMain : Player
{
	public override void _Ready()
	{
		base._Ready();

		// Adiciona ao grupo para enemies encontrarem
		AddToGroup("player");
		GD.Print($"PlayerMain adicionado ao grupo 'player' na posição: {GlobalPosition}");
	}
}
