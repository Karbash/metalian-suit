using Godot;

/// <summary>
/// Controla toda a física de uma entidade.
/// Sistema simples e confiável com tratamento de erros.
/// </summary>
public class PhysicsController
{
	private readonly CharacterBody2D body;
	private readonly EntityData data;

	public Vector2 Velocity { get; set; }

	public PhysicsController(CharacterBody2D body, EntityData data)
	{
		this.body = body ?? throw new System.ArgumentNullException(nameof(body));
		this.data = data ?? throw new System.ArgumentNullException(nameof(data));
	}

	/// <summary>
	/// Aplica física básica: gravidade, limites de velocidade
	/// </summary>
	public void ApplyPhysics(double delta)
	{
		try
		{
			// Gravidade sempre se aplica
			if(!body.IsOnFloor())
			{
				Velocity = new Vector2(Velocity.X, Velocity.Y + data.Gravity);
			}

			// Limites de velocidade para estabilidade
			Velocity = new Vector2(
				Mathf.Clamp(Velocity.X, -data.MaxSpeed, data.MaxSpeed),
				Mathf.Clamp(Velocity.Y, -data.MaxFallSpeed, data.MaxFallSpeed)
			);

			// Aplica ao corpo do Godot
			body.Velocity = Velocity;
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"PhysicsController: Erro em ApplyPhysics: {e.Message}");
			Velocity = Vector2.Zero;
			body.Velocity = Vector2.Zero;
		}
	}

	/// <summary>
	/// Movimento horizontal básico
	/// Respeita estado de stun NES
	/// </summary>
	public void Move(float direction)
	{
		try
		{
			// NES: Sem movimento durante knockback/stun
			if(body is Entity entity && entity.IsStunned)
				return;

			float speed = direction * data.MoveSpeed;
			Velocity = new Vector2(speed, Velocity.Y);
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"PhysicsController: Erro em Move: {e.Message}");
		}
	}

	/// <summary>
	/// Para movimento horizontal
	/// </summary>
	public void StopX()
	{
		try
		{
			Velocity = new Vector2(0, Velocity.Y);
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"PhysicsController: Erro em StopX: {e.Message}");
		}
	}

	/// <summary>
	/// Pulo se estiver no chão
	/// NES: Sem pulo durante stun
	/// </summary>
	public void Jump(float force)
	{
		try
		{
			// NES: Sem pulo durante knockback
			if(body is Entity entity && entity.IsStunned)
				return;

			if(body.IsOnFloor())
			{
				Velocity = new Vector2(Velocity.X, force);
			}
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"PhysicsController: Erro em Jump: {e.Message}");
		}
	}

	/// <summary>
	/// Reduz velocidade do pulo quando solta o botão
	/// </summary>
	public void CutJump()
	{
		try
		{
			if(Velocity.Y < 0)
			{
				Velocity = new Vector2(Velocity.X, Velocity.Y * 0.5f);
			}
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"PhysicsController: Erro em CutJump: {e.Message}");
		}
	}

	/// <summary>
	/// Aplica knockback - sobrescreve velocidade atual
	/// </summary>
	public void ApplyKnockback(Vector2 knockbackVelocity)
	{
		try
		{
			Velocity = knockbackVelocity;
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"PhysicsController: Erro em ApplyKnockback: {e.Message}");
		}
	}
}
