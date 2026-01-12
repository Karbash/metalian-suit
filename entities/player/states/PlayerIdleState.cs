public class PlayerIdleState : GroundedState
{
	protected override string AnimationName => "idle";
	
	public PlayerIdleState(Entity entity) : base(entity) { }
}
