using Godot;

/// <summary>
/// Torrão estático que atira em intervalo fixo.
/// Startup → Fire → Cooldown.
/// </summary>
public partial class Turret : Enemy
{
	[Export] public PackedScene ProjectileScene;
	
	protected override int GetAIMove()
	{
		return 0; // Não se move
	}
	
	protected override void SetupStateMachine()
	{
		stateMachine.AddState("dormant", new TurretDormantState(this));
		stateMachine.AddState("startup", new TurretStartupState(this));
		stateMachine.AddState("fire", new TurretFireState(this));
		stateMachine.AddState("cooldown", new TurretCooldownState(this));
		stateMachine.AddState("dead", new WalkerDeadState(this));
		
		stateMachine.ChangeState("dormant");
	}
	
	public void FireProjectile()
	{
		if(ProjectileScene == null) return;
		
		var projectile = ProjectileScene.Instantiate<Node2D>();
		projectile.GlobalPosition = GlobalPosition + new Vector2(FacingRight ? 10 : -10, 0);
		GetTree().Root.AddChild(projectile);
		
		// Configurar direção do projétil
		if(projectile is Projectile proj)
		{
			proj.SetDirection(FacingRight ? 1 : -1);
		}
	}
}
