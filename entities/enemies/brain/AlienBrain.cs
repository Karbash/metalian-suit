using Godot;

/// <summary>
/// Power-up que vira inimigo.
/// Estilo Metroid - persegue player por tempo limitado e explode.
/// </summary>

public partial class AlienBrain : Enemy
{
	[Export] public PackedScene ExplosionScene;
	
	private int hopCount = 0;
	private const int MAX_HOPS = 5;
	
	protected override int GetAIMove()
	{
		if(player == null) return 0;
		return player.GlobalPosition.X < GlobalPosition.X ? -1 : 1;
	}
	
	protected override void SetupStateMachine()
	{
		stateMachine.AddState("chase", new BrainChaseState(this));
		stateMachine.AddState("hop", new BrainHopState(this));
		stateMachine.AddState("explode", new BrainExplodeState(this));
		
		stateMachine.ChangeState("chase");
	}
	
	public void IncrementHop()
	{
		hopCount++;
		
		if(hopCount >= MAX_HOPS)
		{
			stateMachine.ChangeState("explode");
		}
	}
	
	public void Explode()
	{
		if(ExplosionScene != null)
		{
			var explosion = ExplosionScene.Instantiate<Node2D>();
			explosion.GlobalPosition = GlobalPosition;
			GetTree().Root.AddChild(explosion);
		}
		
		QueueFree();
	}
	
	protected override void OnDied()
	{
		// Brain não dropa brain
		Explode();
	}
}
