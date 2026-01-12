using Godot;

public class TurretStartupState : State
{
	protected override string AnimationName => "startup";
	
	private FrameTimer startupTimer;
	
	public TurretStartupState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		
		// Vira para direção do player
		if(intent.TargetPosition.X < entity.GlobalPosition.X)
			entity.FacingRight = false;
		else
			entity.FacingRight = true;
		
		startupTimer = new FrameTimer(20); // ~0.33s startup
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		startupTimer.Tick();
		
		if(startupTimer.Done)
		{
			ChangeState("fire");
		}
	}
}
