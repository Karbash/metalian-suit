public class PlayerWalkState : GroundedState
{
	protected override string AnimationName => "walk";
	
	public PlayerWalkState(Entity entity) : base(entity) { }
}
