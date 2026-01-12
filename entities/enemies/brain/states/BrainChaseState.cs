using Godot;

public class BrainChaseState : State
{
	protected override string AnimationName => "float";
	
	private FrameTimer chaseTimer;
	
	public BrainChaseState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		chaseTimer = new FrameTimer(30); // ~0.5s perseguindo
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		chaseTimer.Tick();
		
		// Flutua em direção ao player
		entity.FacingRight = intent.MoveX > 0;
		physics.Move(intent.MoveX * 0.6f); // Mais lento que walker
		
		if(chaseTimer.Done)
		{
			ChangeState("hop");
		}
	}
}
