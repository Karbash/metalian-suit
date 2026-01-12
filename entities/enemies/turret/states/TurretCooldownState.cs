using Godot;

public class TurretCooldownState : State
{
	protected override string AnimationName => "cooldown";
	
	private FrameTimer cooldownTimer;
	
	public TurretCooldownState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		cooldownTimer = new FrameTimer(60); // 1 segundo
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		cooldownTimer.Tick();
		
		if(cooldownTimer.Done)
		{
			ChangeState("dormant");
		}
	}
}
