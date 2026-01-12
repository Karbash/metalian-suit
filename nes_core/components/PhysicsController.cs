using Godot;

	public class PhysicsController
	{
		private CharacterBody2D body;
		private EntityData data;

		public Vector2 Velocity { get; set; }

	public PhysicsController(CharacterBody2D body, EntityData data)
	{
		this.body = body;
		this.data = data;
	}
	
	public void ApplyPhysics(double delta)
	{
		// Gravidade (sempre aplica, mesmo em knockback)
		if(!body.IsOnFloor())
			Velocity = new Vector2(Velocity.X, Velocity.Y + data.Gravity);

		// Clamp
		Velocity = new Vector2(
			Mathf.Clamp(Velocity.X, -data.MaxSpeed, data.MaxSpeed),
			Mathf.Clamp(Velocity.Y, -data.MaxFallSpeed, data.MaxFallSpeed)
		);

		body.Velocity = Velocity;
	}
	
	public void Move(float direction)
	{
		// Checa se pode mover (knockback ativo)
		if(body is Entity entity)
		{
			var knockbackController = entity.GetNodeOrNull<KnockbackController>("KnockbackController");
			if(knockbackController != null && !knockbackController.CanMove())
			{
				return;
			}
		}

		Velocity = new Vector2(direction * data.MoveSpeed, Velocity.Y);
	}

	public void StopX()
	{
		Velocity = new Vector2(0, Velocity.Y);
	}

	public void Jump(float force)
	{
		// Não pode pular durante knockback
		if(body is Entity entity)
		{
			var knockbackController = entity.GetNodeOrNull<KnockbackController>("KnockbackController");
			if(knockbackController != null && knockbackController.IsInKnockback)
			{
				return;
			}
		}

		if(!body.IsOnFloor()) return;
		Velocity = new Vector2(Velocity.X, force);
	}
	
	public void CutJump()
	{
		if(Velocity.Y < 0)
			Velocity = new Vector2(Velocity.X, Velocity.Y * 0.5f);
	}
	
}
