using Godot;

/// <summary>
/// AlienBrain NES-style: Persegue player, pula e explode.
/// Comportamento simples: segue → pula → explode.
/// </summary>
public partial class AlienBrain : Enemy
{
	[Export] public PackedScene ExplosionScene;

	private int hopCount = 0;
	private const int MAX_HOPS = 5;
	private float chaseTimer;
	private bool isHopping;

	protected override void UpdateAI(double delta)
	{
		if(player == null) return;

		chaseTimer += (float)delta;

		if(isHopping)
		{
			// Durante pulo, não faz nada
			if(IsOnFloor())
			{
				isHopping = false;
				hopCount++;

				if(hopCount >= MAX_HOPS)
				{
					Explode();
					return;
				}
			}
		}
		else
		{
			// Persegue player
			float dirToPlayer = player.GlobalPosition.X > GlobalPosition.X ? 1f : -1f;
			direction = dirToPlayer;
			FacingRight = direction > 0;

			// Pula periodicamente
			if(chaseTimer >= 1.5f) // Pula a cada 1.5s
			{
				isHopping = true;
				physicsController.Jump(data.JumpForce);
				chaseTimer = 0;
			}
		}
	}

	private void Explode()
	{
		if(ExplosionScene != null)
		{
			var explosion = ExplosionScene.Instantiate<Node2D>();
			explosion.GlobalPosition = GlobalPosition;
			GetTree().Root.AddChild(explosion);
		}

		QueueFree();
	}

	protected override void OnDeath()
	{
		// Brain não dropa brain, apenas explode
		Explode();
	}
}
