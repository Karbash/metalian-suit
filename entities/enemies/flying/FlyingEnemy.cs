using Godot;

public partial class FlyingEnemy : Enemy
{
	protected override int GetAIMove()
	{
		if(player == null) return 0;
		return player.GlobalPosition.X > GlobalPosition.X ? 1 : -1;
	}
	
	protected override void SetupStateMachine()
	{
		stateMachine.AddState("patrol", new FlyingPatrolState(this));
		stateMachine.AddState("dead", new WalkerDeadState(this));
		stateMachine.ChangeState("patrol");
	}
}

public class FlyingPatrolState : State
{
	protected override string AnimationName => "fly";
	
	public FlyingPatrolState(Entity entity) : base(entity) { }
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		physics.Velocity = new Vector2(
			intent.MoveX * entity.data.MoveSpeed,
			Mathf.Sin((float)Time.GetTicksMsec() / 200f) * 20f
		);
	}
}
