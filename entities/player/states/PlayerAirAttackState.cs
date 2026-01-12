using Godot;

	/// <summary>
/// Estado de ataque aéreo.
/// Mantém momentum do pulo mas para movimento horizontal.
/// Hitbox diferente do ataque terrestre.
/// </summary>

public class PlayerAirAttackState : State
{
	protected override string AnimationName => "attack"; // Mesma animação, mas comportamento diferente

	private FrameTimer attackTimer;
	private AttackData attackData;
	private bool attackCompleted;

	public PlayerAirAttackState(Entity entity) : base(entity) { }

	public override void Enter()
	{
		base.Enter();

		attackCompleted = false;

		// Mantém velocidade vertical (momentum do pulo), mas para horizontal
		physics.Velocity = new Vector2(0, physics.Velocity.Y);

		// Setup attack data para ar (hitbox diferente)
		attackData = new AttackData
		{
			AttackName = "air_attack", // Nome único para ataque aéreo
			Damage = entity.data.AttackDamage,
			HitboxFrames = new()
			{
				new HitboxFrameData
				{
					Frame = 1, // Ajustado para frame 1 da animação
					Offset = new Vector2(16, 0), // Mesmo offset do ataque terrestre
					Size = new Vector2(12, 16)   // Mesmo tamanho do ataque terrestre
				}
			},
			StartupFrames = 1,  // Sincronizado com animação
			ActiveFrames = 2,   // Hitbox ativa por tempo similar
			RecoveryFrames = 1  // Recovery mínimo
		};

		combat.RegisterAttack("air_attack", attackData); // Nome único

		// Ativa hitbox imediatamente no início do ataque aéreo
		if(attackData.HitboxFrames != null && attackData.HitboxFrames.Count > 0)
		{
			var hitboxFrame = attackData.HitboxFrames[0];
			var offset = hitboxFrame.Offset;
			if(!entity.FacingRight)
				offset.X = -offset.X;

			combat.SetHitboxActive(true, offset, hitboxFrame.Size, attackData.Damage);
		}

		attackTimer = new FrameTimer(attackData.TotalFrames);

		sprite.AnimationComplete += OnAttackComplete;

		GD.Print($"Player iniciou ataque aéreo: Dano={attackData.Damage}");
	}

	public override void Update(double delta)
	{
		base.Update(delta);

		attackTimer.Tick();

		// Verifica fim do ataque
		if(!attackCompleted && attackTimer.Done)
		{
			OnAttackFinished();
			return;
		}

		// Mantém movimento vertical (gravidade/gravity)
		// Movimento horizontal parado durante ataque

		// Pousou durante ataque?
		if(intent.OnFloor)
		{
			// Termina ataque e vai para idle/walk
			OnAttackFinished();
			return;
		}
	}

	private void OnAttackComplete(string animName)
	{
		if(animName == "attack" && !attackCompleted)
		{
			OnAttackFinished();
		}
	}

	private void OnAttackFinished()
	{
		attackCompleted = true;

		if(intent.OnFloor)
		{
			// Pousou - vai para estado terrestre
			ChangeState(intent.MoveX != 0 ? "walk" : "idle");
		}
		else
		{
			// Ainda no ar - volta para air
			ChangeState("air");
		}

		GD.Print("Ataque aéreo do player finalizado");
	}

	public override void Exit()
	{
		sprite.AnimationComplete -= OnAttackComplete;
	}
}