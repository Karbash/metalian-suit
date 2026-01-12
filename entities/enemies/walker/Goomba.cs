using Godot;

/// <summary>
/// Goomba NES-style: Patrulla simples, flip em obstáculos.
/// Comportamento direto e previsível.
/// </summary>
public partial class Goomba : Enemy
{
	protected override void UpdateAI(double delta)
	{
		// NES: Walker simples - anda até bater em algo
		if(IsOnWall() || !IsGroundAhead() || IsObstacleAhead())
		{
			FlipDirection();
		}
	}
}
