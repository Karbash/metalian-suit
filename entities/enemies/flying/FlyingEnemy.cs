using Godot;

/// <summary>
/// FlyingEnemy NES-style: Voa em ondas, persegue o player.
/// Movimento fluido e previsível.
/// </summary>
public partial class FlyingEnemy : Enemy
{
	protected override void UpdateAI(double delta)
	{
		if(player == null) return;

		// NES: Persegue horizontalmente, movimento ondulado vertical
		float horizontalDir = player.GlobalPosition.X > GlobalPosition.X ? 1f : -1f;
		float verticalWave = Mathf.Sin((float)Time.GetTicksMsec() / 200f) * 20f;

		// Movimento direto
		Velocity = new Vector2(horizontalDir * data.MoveSpeed, verticalWave);

		// Atualizar direção do sprite
		if(horizontalDir != direction)
		{
			direction = horizontalDir;
			FacingRight = direction > 0;
		}
	}

	protected override void UpdateAnimation()
	{
		string animation = "fly"; // Flying sempre usa "fly"

		if(spriteController != null)
		{
			var currentAnim = spriteController.GetCurrentAnimation();
			if(currentAnim != animation)
			{
				spriteController.Play(animation, FacingRight);
			}
		}
	}
}
