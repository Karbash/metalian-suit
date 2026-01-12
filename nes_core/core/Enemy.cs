using Godot;

/// <summary>
/// Enemy NES-style: Comportamento direto e previsível.
/// Sem estados complexos - apenas IA básica.
/// </summary>
public abstract partial class Enemy : Entity
{
	[Export] public BrainDropData BrainDrop;

	protected Player player;
	protected float direction = -1f; // -1 = esquerda, 1 = direita

	public override void _Ready()
	{
		base._Ready();

		// Adiciona ao grupo de damage on contact
		AddToGroup("damage_on_contact");

		// Busca player
		player = GetTree().GetFirstNodeInGroup("player") as Player;
	}

	protected override void UpdateBehavior(double delta)
	{
		// NES: Sem comportamento se estiver stunned
		if(IsStunned) return;

		// IA básica
		UpdateAI(delta);

		// Movimento
		physicsController.Move(direction);

		// Animação simples
		UpdateAnimation();
	}

	/// <summary>
	/// IA específica de cada enemy
	/// </summary>
	protected abstract void UpdateAI(double delta);

	/// <summary>
	/// Inverte direção (usado por walkers)
	/// </summary>
	public void FlipDirection()
	{
		direction *= -1;
		FacingRight = direction > 0;
	}

	/// <summary>
	/// Verifica se há obstáculo à frente
	/// </summary>
	protected bool IsObstacleAhead(float distance = 16f)
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var query = PhysicsRayQueryParameters2D.Create(
			GlobalPosition + new Vector2(0, -8),
			GlobalPosition + new Vector2(direction * distance, -8)
		);
		query.CollisionMask = 4; // World layer

		var result = spaceState.IntersectRay(query);
		return result.Count > 0 && (GodotObject)result["collider"] != this;
	}

	/// <summary>
	/// Verifica se há chão à frente
	/// </summary>
	protected bool IsGroundAhead(float distance = 20f)
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var query = PhysicsRayQueryParameters2D.Create(
			GlobalPosition,
			GlobalPosition + new Vector2(direction * distance, 20f)
		);
		query.CollisionMask = 4; // World layer

		var result = spaceState.IntersectRay(query);
		return result.Count > 0;
	}

	protected virtual void UpdateAnimation()
	{
		string animation = "walk"; // Default

		if(spriteController != null)
		{
			var currentAnim = spriteController.GetCurrentAnimation();
			if(currentAnim != animation)
			{
				spriteController.Play(animation, FacingRight);
			}
		}
	}

	protected override void OnDeath()
	{
		// Drop brain
		if(BrainDrop != null && GD.Randf() <= BrainDrop.DropChance)
		{
			BrainDrop.Spawn(GlobalPosition);
		}

		// Remove enemy
		QueueFree();
	}
}
