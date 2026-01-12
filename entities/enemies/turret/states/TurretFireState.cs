using Godot;

public class TurretFireState : State
{
	protected override string AnimationName => "fire";
	
	private bool hasFired;
	
	public TurretFireState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		hasFired = false;
		
		sprite.AnimationComplete += OnFireComplete;
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		// Dispara no frame específico da animação
		if(!hasFired && sprite.GetCurrentFrame() == 2)
		{
			(entity as Turret)?.FireProjectile();
			hasFired = true;
		}
	}
	
	private void OnFireComplete(string animName)
	{
		if(animName == "fire")
		{
			ChangeState("cooldown");
		}
	}
	
	public override void Exit()
	{
		sprite.AnimationComplete -= OnFireComplete;
	}
}
