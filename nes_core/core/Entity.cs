using Godot;

/// <summary>
/// Base de todas as entidades (Player, Enemy).
/// Calcula FrameIntent uma vez por frame.
/// </summary>

public abstract partial class Entity : CharacterBody2D
{
	[Export] public EntityData data;
	
	public StateMachine stateMachine;
	public PhysicsController physicsController;

	protected SpriteController spriteController;
	protected CombatController combatController;
	protected KnockbackController knockbackController; // NOVO
	protected InputController input;
	
	public bool FacingRight { get; set; } = true;
	
	private FrameIntent currentIntent;
	
	public override void _Ready()
	{
		spriteController = GetNode<SpriteController>("SpriteController");
		combatController = GetNode<CombatController>("CombatController");
		knockbackController = GetNodeOrNull<KnockbackController>("KnockbackController"); // NOVO
		input = GetNode<InputController>("/root/InputController");
		physicsController = new PhysicsController(this, data);

		stateMachine = new StateMachine();
		SetupStateMachine();

		combatController.Damaged += OnDamaged;
		combatController.Died += OnDied;

		// NOVO: Escuta knockback
		if(knockbackController != null)
		{
			knockbackController.KnockbackStarted += OnKnockbackStarted;
			knockbackController.KnockbackEnded += OnKnockbackEnded;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		currentIntent = BuildIntent();
		stateMachine.Update(delta);
		physicsController.ApplyPhysics(delta);
		MoveAndSlide();
	}
	
	protected abstract void SetupStateMachine();
	
	/// <summary>
	/// Constrói a intenção do frame.
	/// Player: lê input
	/// Enemy: calcula AI
	/// </summary>
	protected abstract FrameIntent BuildIntent();
	
	/// <summary>
	/// Estados pegam a intenção via este método.
	/// </summary>
	public FrameIntent GetIntent() => currentIntent;
	
	protected virtual void OnDamaged(int damage, Vector2 knockbackDir)
	{
		// Knockback agora é gerenciado automaticamente pelo KnockbackController
		// Apenas força transição de estado
		if(combatController.GetCurrentHealth() <= 0)
		{
			stateMachine.ChangeState("dead");
		}
		else
		{
			stateMachine.ChangeState("hurt");
		}
	}

	protected virtual void OnKnockbackStarted()
	{
		// Override em classes derivadas se precisar
	}

	protected virtual void OnKnockbackEnded()
	{
		// Override em classes derivadas se precisar
	}

	
	protected virtual void OnDied()
	{
		stateMachine.ChangeState("dead");
	}
}
