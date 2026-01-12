using Godot;

/// <summary>
/// Controla knockback estilo NES.
/// Integra com PhysicsController e StateMachine.
/// </summary>
public partial class KnockbackController : Node
{
	[Export] public KnockbackData DefaultKnockback;

	private Entity owner;
	private SpriteController sprite;

	// Estado atual
	private bool isInKnockback;
	private FrameTimer stunTimer;
	private FrameTimer invulnerableTimer;
	private KnockbackData currentKnockback;

	// Rotação (Ninja Gaiden style)
	private float rotationSpeed;
	private float currentRotation;

	// Signals
	[Signal] public delegate void KnockbackStartedEventHandler();
	[Signal] public delegate void KnockbackEndedEventHandler();
	[Signal] public delegate void InvulnerabilityEndedEventHandler();

	public bool IsInKnockback => isInKnockback;
	public bool IsInvulnerable => invulnerableTimer != null && !invulnerableTimer.Done;

	public override void _Ready()
	{
		owner = GetParent<Entity>();
		sprite = owner.GetNode<SpriteController>("SpriteController");

		if(DefaultKnockback == null)
		{
			DefaultKnockback = KnockbackData.MegaMan;
		}
	}

    // Busca physicsController de forma lazy
    private PhysicsController GetPhysicsController()
    {
        if(owner != null)
        {
            return owner.physicsController;
        }
        return null;
    }

	public override void _Process(double delta)
	{
		if(!isInKnockback) return;

		// Tick timers
		stunTimer?.Tick();
		invulnerableTimer?.Tick();

		// Rotação (se ativo)
		if(currentKnockback.RotateSprite)
		{
			currentRotation += rotationSpeed * (float)delta;
			sprite.Rotation = currentRotation;
		}

		// Fim do stun
		if(stunTimer != null && stunTimer.Done)
		{
			EndKnockback();
		}

		// Fim da invulnerabilidade
		if(invulnerableTimer != null && invulnerableTimer.Done)
		{
			EmitSignal(SignalName.InvulnerabilityEnded);
			invulnerableTimer = null;
		}
	}

	/// <summary>
	/// Aplica knockback baseado em fonte de dano.
	/// </summary>
	public void ApplyKnockback(Vector2 damageSourcePosition, KnockbackData knockbackData = null)
	{
		if(IsInvulnerable) return;

		var data = knockbackData ?? DefaultKnockback;

		// Nenhum knockback (boss imune, etc)
		if(data.Type == KnockbackType.None) return;

		currentKnockback = data;
		isInKnockback = true;

		// Calcula direção
		Vector2 knockbackDirection = CalculateDirection(damageSourcePosition);

		// Cancela velocidade atual (NES padrão)
		var physicsController = GetPhysicsController();
		if(data.CancelVelocity && physicsController != null)
		{
			physicsController.Velocity = Vector2.Zero;
		}

		// Aplica força
		ApplyKnockbackForce(knockbackDirection, data);

		// Setup timers
		stunTimer = new FrameTimer(data.StunFrames);
		invulnerableTimer = new FrameTimer(data.InvulnerableFrames);

		// Rotação (Ninja Gaiden)
		if(data.RotateSprite)
		{
			rotationSpeed = 720f; // 2 rotações por segundo
			currentRotation = 0f;
		}

		EmitSignal(SignalName.KnockbackStarted);
	}

	/// <summary>
	/// Calcula direção do knockback baseado na fonte de dano.
	/// NES: sempre oposto à fonte.
	/// </summary>
	private Vector2 CalculateDirection(Vector2 sourcePosition)
	{
		float horizontalDir = Mathf.Sign(owner.GlobalPosition.X - sourcePosition.X);

		// Se colidiu exatamente em cima, usa direção que está olhando
		if(Mathf.Abs(horizontalDir) < 0.1f)
		{
			horizontalDir = owner.FacingRight ? -1f : 1f;
		}

		return new Vector2(horizontalDir, -1f).Normalized();
	}

	/// <summary>
	/// Aplica força de knockback.
	/// </summary>
	private void ApplyKnockbackForce(Vector2 direction, KnockbackData data)
	{
		var physicsController = GetPhysicsController();
		if(physicsController == null) return;

		switch(data.Type)
		{
			case KnockbackType.Light:
				// Metroid: horizontal apenas
				physicsController.Velocity = new Vector2(
					direction.X * data.HorizontalForce,
					physicsController.Velocity.Y
				);
				break;

			case KnockbackType.Standard:
				// Mega Man: diagonal fixo
				physicsController.Velocity = new Vector2(
					direction.X * data.HorizontalForce,
					data.VerticalForce
				);
				break;

			case KnockbackType.Heavy:
				// Castlevania: forte diagonal
				physicsController.Velocity = new Vector2(
					direction.X * data.HorizontalForce,
					data.VerticalForce
				);
				break;

			case KnockbackType.Launch:
				// Ninja Gaiden: lança longe
				physicsController.Velocity = new Vector2(
					direction.X * data.HorizontalForce,
					data.VerticalForce
				);
				break;

			case KnockbackType.Stun:
				// Boss stagger: apenas paralisa
				physicsController.Velocity = Vector2.Zero;
				break;
		}
	}

	/// <summary>
	/// Termina knockback (mas invulnerabilidade continua).
	/// </summary>
	private void EndKnockback()
	{
		isInKnockback = false;
		stunTimer = null;

		// Reseta rotação
		if(currentKnockback.RotateSprite)
		{
			sprite.Rotation = 0f;
			currentRotation = 0f;
		}

		EmitSignal(SignalName.KnockbackEnded);
	}

	/// <summary>
	/// Força fim do knockback (transição de estado forçada).
	/// </summary>
	public void ForceEnd()
	{
		EndKnockback();
		invulnerableTimer = null;
	}

	/// <summary>
	/// Checa se pode executar ação durante knockback.
	/// </summary>
	public bool CanMove()
	{
		if(!isInKnockback) return true;
		return currentKnockback.AllowAirControl;
	}

	public bool CanAttack()
	{
		if(!isInKnockback) return true;
		return currentKnockback.AllowAttack;
	}
}
