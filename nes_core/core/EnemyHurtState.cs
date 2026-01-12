using Godot;

/// <summary>
/// Estado hurt genérico para enemies.
/// Mantém knockback e volta ao estado anterior.
/// </summary>

public class EnemyHurtState : State
{
	protected override string AnimationName => null; // Mantém animação atual

	private FrameTimer hurtTimer;
	private string previousState;

	public EnemyHurtState(Entity entity) : base(entity) { }

	public override void Enter()
	{
		base.Enter();

		// Salva estado anterior para voltar depois
		var enemy = entity as Enemy;
		previousState = enemy != null ? enemy.GetPreviousState() : "patrol";

		// Knockback já foi aplicado pelo PhysicsController
		hurtTimer = new FrameTimer(15); // Tempo reduzido para hurt mais rápido

		// Reduzir movimento lateral durante hurt para evitar deslizar
		physics.Velocity = new Vector2(physics.Velocity.X * 0.3f, physics.Velocity.Y);
	}

	public override void Update(double delta)
	{
		base.Update(delta);

		hurtTimer.Tick();

		// Sai do hurt quando timer acabar (não depende mais do knockback)
		if(hurtTimer.Done)
		{
			ChangeState(previousState);
		}
	}
}