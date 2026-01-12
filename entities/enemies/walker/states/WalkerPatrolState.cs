using Godot;

public class WalkerPatrolState : State
{
	protected override string AnimationName => "walk";
	
	private Goomba goomba;
	
	public WalkerPatrolState(Entity entity) : base(entity) 
	{
		goomba = entity as Goomba;
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);

		physics.Move(intent.MoveX);

		// Detecta parede, borda ou obstáculos
		if(intent.OnWall || !IsGroundAhead() || IsObstacleAhead())
		{
			goomba.FlipDirection();
		}
	}

	/// <summary>
	/// Verifica se há obstáculos à frente (além de chão)
	/// </summary>
	private bool IsObstacleAhead()
	{
		var spaceState = entity.GetWorld2D().DirectSpaceState;
		var query = PhysicsRayQueryParameters2D.Create(
			entity.GlobalPosition + new Vector2(0, -8), // Centro do corpo
			entity.GlobalPosition + new Vector2(intent.MoveX * 16, -8) // 16 pixels à frente
		);
		query.CollisionMask = 4; // World layer (parede/chão)
		query.Exclude = new Godot.Collections.Array<Rid> { entity.GetRid() }; // Exclui a si mesmo

		var result = spaceState.IntersectRay(query);
		return result.Count > 0 && (GodotObject)result["collider"] != entity; // Há colisão e não é consigo mesmo
	}
	
	private bool IsGroundAhead()
	{
		var spaceState = entity.GetWorld2D().DirectSpaceState;
		var query = PhysicsRayQueryParameters2D.Create(
			entity.GlobalPosition,
			entity.GlobalPosition + new Vector2(intent.MoveX * 20, 20)
		);
		query.CollisionMask = 4; // Layer 3 (World)
		
		var result = spaceState.IntersectRay(query);
		return result.Count > 0;
	}
}
