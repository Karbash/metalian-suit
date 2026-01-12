using Godot;

public class TurretDormantState : State
{
	protected override string AnimationName => "idle";
	
	private FrameTimer dormantTimer;
	
	public TurretDormantState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
		dormantTimer = new FrameTimer(120); // 2 segundos
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		dormantTimer.Tick();
		
		// Detecta player próximo ou timer esgotado
		if(dormantTimer.Done || (intent.DistanceToTarget < 150))
		{
			ChangeState("startup");
		}
	}
}
