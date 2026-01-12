using Godot;

/// <summary>
/// Estado de ataque com Startup/Active/Recovery.
/// NÃO CANCELÁVEL (ConstraintCore.AllowAttackCancel = false)
///
/// Timing NES típico:
/// - Startup: 3 frames (preparação)
/// - Active: 5 frames (hitbox ativa)
/// - Recovery: 8 frames (recuperação)
/// Total: 16 frames
/// </summary>

public class PlayerAttackState : State
{
	protected override string AnimationName => "attack";

	private FrameTimer attackTimer;
	private AttackData attackData;
	private bool attackCompleted;
	
	public PlayerAttackState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();

		attackCompleted = false;

		// Para movimento durante ataque (NES style)
		physics.Velocity = new Vector2(0, physics.Velocity.Y);
		
		// Setup attack data com hitbox completa
		attackData = new AttackData
		{
			AttackName = "attack",
			Damage = entity.data.AttackDamage,
			HitboxFrames = new()
			{
				new HitboxFrameData
				{
					Frame = 0, // Frame único da animação
					Offset = new Vector2(16, 0), // À frente do player
					Size = new Vector2(12, 16) // Hitbox maior para ataque
				}
			},
			StartupFrames = 1,  // Reduzido para sincronizar com animação
			ActiveFrames = 2,   // Hitbox ativa por menos tempo
			RecoveryFrames = 1  // Recovery mínimo
		};
		
		// Registra ataque no CombatController
		combat.RegisterAttack("attack", attackData);

		// Ativa hitbox imediatamente no início do ataque
		if(attackData.HitboxFrames != null && attackData.HitboxFrames.Count > 0)
		{
			var hitboxFrame = attackData.HitboxFrames[0];
			var offset = hitboxFrame.Offset;
			if(!entity.FacingRight)
				offset.X = -offset.X;

			combat.SetHitboxActive(true, offset, hitboxFrame.Size, attackData.Damage);
		}

		attackTimer = new FrameTimer(attackData.TotalFrames);

		// Escuta fim da animação para transição de estado
		sprite.AnimationComplete += OnAttackComplete;

		GD.Print($"Player iniciou ataque: Dano={attackData.Damage}, TotalFrames={attackData.TotalFrames}");
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);

		attackTimer.Tick();

		// Verifica se ataque terminou pelo timer (segurança)
		if(!attackCompleted && attackTimer.Done)
		{
			attackCompleted = true;
			OnAttackFinished();
		}

		// NES: ataque não cancela (mesmo que pressione outro botão)
		if(!ConstraintCore.AllowAttackCancel)
		{
			// Travado até animação terminar
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

		// Transição baseada no estado atual
		if(intent.OnFloor)
		{
			ChangeState(intent.MoveX != 0 ? "walk" : "idle");
		}
		else
		{
			ChangeState("air");
		}

		GD.Print("Ataque do player finalizado");
	}
	
	public override void Exit()
	{
		sprite.AnimationComplete -= OnAttackComplete;
	}
}
