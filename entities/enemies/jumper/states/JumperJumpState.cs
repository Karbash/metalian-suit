using Godot;

public class JumperJumpState : State
{
	protected override string AnimationName => "jump";
	
	public JumperJumpState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		
		// Pulo fixo (NES não tinha variação)
		physics.Velocity = new Vector2(0, -150f);
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		// Pousou
		if(intent.OnFloor && physics.Velocity.Y >= 0)
		{
			ChangeState("land");
		}
	}
}
