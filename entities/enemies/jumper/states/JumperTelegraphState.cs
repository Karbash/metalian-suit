using Godot;

public class JumperTelegraphState : State
{
	protected override string AnimationName => "telegraph";
	
	private FrameTimer telegraphTimer;
	
	public JumperTelegraphState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		telegraphTimer = new FrameTimer(15); // ~0.25s aviso
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		telegraphTimer.Tick();
		
		if(telegraphTimer.Done)
		{
			ChangeState("jump");
		}
	}
}
