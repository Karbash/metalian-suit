using Godot;

/// <summary>
/// Inimigo estilo Metroid que pula periodicamente.
/// Dano apenas por contato.
/// </summary>

public partial class Jumper : Enemy
{
	protected override int GetAIMove()
	{
		return 0; // Não anda, apenas pula
	}
	
	protected override void SetupStateMachine()
	{
		stateMachine.AddState("idle", new JumperIdleState(this));
		stateMachine.AddState("telegraph", new JumperTelegraphState(this));
		stateMachine.AddState("jump", new JumperJumpState(this));
		stateMachine.AddState("land", new JumperLandState(this));
		stateMachine.AddState("hurt", new PlayerHurtState(this));
		stateMachine.AddState("dead", new WalkerDeadState(this));
		
		stateMachine.ChangeState("idle");
	}
}
