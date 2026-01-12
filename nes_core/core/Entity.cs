using Godot;

/// <summary>
/// Base sólida para todas as entidades (Player, Enemy).
/// Sistema confiável com tratamento de erros.
/// </summary>
	/// <summary>
	/// Entidade NES-style: Simples, direta, confiável.
	/// Sem estados complexos - apenas comportamento básico.
	/// </summary>
	public abstract partial class Entity : CharacterBody2D
	{
		[Export] public EntityData data;

		// Componentes essenciais
		public PhysicsController physicsController;
		public SpriteController spriteController;
		public CombatController combatController;

		// Estado simples NES
		public bool FacingRight { get; set; } = true;
		public bool IsInvulnerable { get; set; }
		public bool IsStunned { get; set; } // Knockback ativo
		public int Health { get; set; }

		// Input (para Player)
		protected InputController input;

	public override void _Ready()
	{
		Health = data?.MaxHealth ?? 1;

		// Componentes essenciais
		spriteController = GetNode<SpriteController>("SpriteController");
		combatController = GetNode<CombatController>("CombatController");

		// Física
		physicsController = new PhysicsController(this, data);

		// Eventos de combate
		combatController.Damaged += OnDamaged;
		combatController.Died += OnDied;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Atualiza comportamento básico
		UpdateBehavior(delta);

		// Aplica física
		physicsController.ApplyPhysics(delta);

		// Movimento
		MoveAndSlide();
	}

	/// <summary>
	/// Atualiza comportamento básico da entidade
	/// Chamado todo frame antes da física
	/// </summary>
	protected abstract void UpdateBehavior(double delta);

	/// <summary>
	/// Chamado quando a entidade morre
	/// </summary>
	protected virtual void OnDeath()
	{
		// Override nas subclasses
	}

	/// <summary>
	/// Chamado quando toma dano
	/// </summary>
	protected virtual void OnDamaged(int damage, Vector2 knockbackDir)
	{
		// NES-STYLE: Knockback imediato sempre
		if(!IsInvulnerable)
		{
			physicsController.ApplyKnockback(knockbackDir);
			StartInvulnerability();
		}

		// Sem estados complexos - apenas morte
		if(Health <= 0)
		{
			OnDeath();
		}
	}

	private void StartInvulnerability()
	{
		IsInvulnerable = true;
		IsStunned = true; // Knockback ativo = stunned

		// Flash visual NES
		spriteController.StartHurtFlash(data.InvulnerabilityTime);

		// Timer de invulnerabilidade
		GetTree().CreateTimer(data.InvulnerabilityTime).Timeout += () => {
			IsInvulnerable = false;
			IsStunned = false;
		};
	}

	/// <summary>
	/// Chamado quando morre
	/// </summary>
	protected virtual void OnDied()
	{
		// Override nas subclasses
	}

	/// <summary>
	/// Método direto para tomar dano
	/// </summary>
	public void TakeDamage(int damage, Vector2 knockbackDir)
	{
		try
		{
			Health = Mathf.Max(0, Health - damage);
			OnDamaged(damage, knockbackDir);
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"{Name}: Erro em TakeDamage: {e.Message}");
		}
	}

	/// <summary>
	/// Verifica se pode executar ações
	/// </summary>
	public virtual bool CanAct() => !IsInvulnerable;
}
