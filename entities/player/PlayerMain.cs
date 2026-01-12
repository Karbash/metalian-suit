using Godot;

public partial class PlayerMain : Player
{
	public override void _Ready()
	{
		base._Ready();

		// Adiciona ao grupo para enemies encontrarem
		AddToGroup("player");
		GD.Print($"PlayerMain adicionado ao grupo 'player' na posição: {GlobalPosition}");
	}
	
	protected override void SetupStateMachine()
	{
		stateMachine.AddState("idle", new PlayerIdleState(this));
		stateMachine.AddState("walk", new PlayerWalkState(this));
		stateMachine.AddState("air", new PlayerAirState(this));
		stateMachine.AddState("attack", new PlayerAttackState(this));
		stateMachine.AddState("air_attack", new PlayerAirAttackState(this));
		stateMachine.AddState("hurt", new PlayerHurtState(this));
		stateMachine.AddState("dead", new PlayerDeadState(this));

		stateMachine.ChangeState("idle");
	}
}
