using Godot;

public class BrainExplodeState : State
{
	protected override string AnimationName => "explode";
	
	private AlienBrain brain;
	private FrameTimer explodeTimer;
	
	public BrainExplodeState(Entity entity) : base(entity) 
	{
		brain = entity as AlienBrain;
	}
	
	public override void Enter()
	{
		base.Enter();
		
		physics.StopX();
		explodeTimer = new FrameTimer(15); // ~0.25s antes de explodir
		
		sprite.AnimationComplete += OnExplodeComplete;
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		explodeTimer.Tick();
	}
	
	private void OnExplodeComplete(string animName)
	{
		if(animName == "explode")
		{
			brain.Explode();
		}
	}
	
	public override void Exit()
	{
		sprite.AnimationComplete -= OnExplodeComplete;
	}
}
