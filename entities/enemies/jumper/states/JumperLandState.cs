using Godot;

public class JumperLandState : State
{
	protected override string AnimationName => "land";
	
	private FrameTimer landTimer;
	
	public JumperLandState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		
		physics.StopX();
		landTimer = new FrameTimer(10); // Recuperação curta
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		landTimer.Tick();
		
		if(landTimer.Done)
		{
			ChangeState("idle");
		}
	}
}
