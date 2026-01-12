using Godot;

/// <summary>
/// Player NES-style: Comportamento direto, sem estados complexos.
/// Movimento + Ataque básicos.
/// </summary>
public abstract partial class Player : Entity
{
	// Estado simples do player
	private bool isAttacking;
	private float attackTimer;

	public override void _Ready()
	{
		base._Ready();
		input = GetNode<InputController>("/root/InputController");
	}

	protected override void UpdateBehavior(double delta)
	{
		// NES: Sem comportamento se estiver stunned
		if(IsStunned) return;

		HandleMovement();
		HandleAttack(delta);
		UpdateAnimation();
	}

	private void HandleMovement()
	{
		float moveX = input.GetHorizontalAxis();

		// Flip direção baseado no movimento
		if(moveX != 0)
		{
			FacingRight = moveX > 0;
		}

		// Movimento horizontal
		physicsController.Move(moveX);

		// Pulo
		if(input.IsJustPressed(InputController.NESButton.A) && IsOnFloor())
		{
			physicsController.Jump(data.JumpForce);
		}
	}

	private void HandleAttack(double delta)
	{
		// Ataque
		if(input.IsJustPressed(InputController.NESButton.B) && !isAttacking)
		{
			StartAttack();
		}

		// Timer de ataque
		if(isAttacking)
		{
			attackTimer -= (float)delta;
			if(attackTimer <= 0)
			{
				EndAttack();
			}
		}
	}

	private void StartAttack()
	{
		isAttacking = true;
		attackTimer = 0.3f; // 18 frames a 60fps

		// Ativar hitbox
		combatController.SetHitboxActive(true,
			new Vector2(FacingRight ? 16 : -16, 0), // À frente
			new Vector2(12, 16), // Tamanho
			1); // Dano

		// Animação
		UpdateAnimation();
	}

	private void EndAttack()
	{
		isAttacking = false;
		combatController.SetHitboxActive(false, Vector2.Zero, Vector2.Zero, 0);
	}

	private void UpdateAnimation()
	{
		string animation = "idle";

		if(isAttacking)
		{
			animation = "attack";
		}
		else if(!IsOnFloor())
		{
			animation = Velocity.Y < 0 ? "jump" : "fall";
		}
		else if(Mathf.Abs(Velocity.X) > 10)
		{
			animation = "walk";
		}

		// Aplicar flip e animação
		if(spriteController != null)
		{
			var currentAnim = spriteController.GetCurrentAnimation();
			if(currentAnim != animation)
			{
				spriteController.Play(animation, FacingRight);
			}
		}
	}

	protected override void OnDeath()
	{
		// Reload scene NES-style
		GetTree().ReloadCurrentScene();
	}

	public void Freeze()
	{
		physicsController.StopX();
		SetProcess(false);
		SetPhysicsProcess(false);
	}

	public void Respawn()
	{
		Health = data.MaxHealth;
		IsInvulnerable = false;
		IsStunned = false;
		SetProcess(true);
		SetPhysicsProcess(true);
		physicsController.StopX();
		GlobalPosition = Vector2.Zero; // Reset position
	}
}
