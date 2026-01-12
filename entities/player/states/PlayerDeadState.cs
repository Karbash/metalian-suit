using Godot;

public class PlayerDeadState : State
{
	protected override string AnimationName => "dead";
	
	private FrameTimer deathTimer;
	
	public PlayerDeadState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		
		// Pulo para cima ao morrer (estilo Mega Man)
		physics.Velocity = new Vector2(0, -100f);
		
		// Desabilita colisões
		entity.SetCollisionLayerValue(1, false);
		entity.SetCollisionMaskValue(1, false);
		
		deathTimer = new FrameTimer(120); // 2 segundos
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		deathTimer.Tick();
		
		if(deathTimer.Done)
		{
			// Reload scene ou game over
			entity.GetTree().ReloadCurrentScene();
		}
	}
}
