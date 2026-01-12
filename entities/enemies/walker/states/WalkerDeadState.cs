using Godot;

public class WalkerDeadState : State
{
	protected override string AnimationName => "dead";
	
	private FrameTimer deathTimer;
	
	public WalkerDeadState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();

		// ✅ PRESERVA KNOCKBACK HORIZONTAL, MAS LIMPA VERTICAL PARA QUEDA SUAVE
		float horizontalVelocity = physics.Velocity.X; // Mantém qualquer knockback horizontal
		physics.Velocity = new Vector2(horizontalVelocity, 20f); // Queda vertical suave

		// Tempo reduzido para desaparecer mais rápido
		deathTimer = new FrameTimer(60); // 1 segundo
	}

	public override void Update(double delta)
	{
		base.Update(delta);

		deathTimer.Tick();

		// Desabilitar colisões imediatamente (sem pulo dramático)
		if(deathTimer.Remaining <= 55 && entity.GetCollisionLayerValue(2))
		{
			entity.SetCollisionLayerValue(2, false); // Enemy layer
			entity.SetCollisionMaskValue(1, false);   // Player layer
			entity.SetCollisionMaskValue(4, false);   // World/Floor layer
		}

		if(deathTimer.Done)
		{
			entity.QueueFree();
		}
	}
}
