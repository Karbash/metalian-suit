using Godot;

public class BrainHopState : State
{
	protected override string AnimationName => "hop";
	
	private AlienBrain brain;
	
	public BrainHopState(Entity entity) : base(entity) 
	{
		brain = entity as AlienBrain;
	}
	
	public override void Enter()
	{
		base.Enter();
		
		// Pequeno pulo
		physics.Velocity = new Vector2(
			intent.MoveX * 80f,
			-100f
		);
		
		brain.IncrementHop();
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		// Volta a perseguir ao pousar
		if(intent.OnFloor && physics.Velocity.Y >= 0)
		{
			ChangeState("chase");
		}
	}
}
