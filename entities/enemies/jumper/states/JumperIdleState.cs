using Godot;

public class JumperIdleState : State
{
	protected override string AnimationName => "idle";
	
	private FrameTimer idleTimer;
	
	public JumperIdleState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		
		physics.StopX();
		idleTimer = new FrameTimer(GD.RandRange(60, 120)); // 1-2 segundos
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		idleTimer.Tick();
		
		if(idleTimer.Done)
		{
			ChangeState("telegraph");
		}
	}
}
