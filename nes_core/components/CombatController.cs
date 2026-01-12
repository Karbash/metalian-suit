using Godot;

/// <summary>
/// Gerencia todo o combate de uma entidade.
/// Sistema simples e confiável.
/// </summary>
public partial class CombatController : Node2D
{
	[Signal] public delegate void DamagedEventHandler(int damage, Vector2 knockbackDir);
	[Signal] public delegate void DiedEventHandler();

	public struct DamageInfo
	{
		public int Damage;
		public Vector2 SourcePosition;
		public float KnockbackForce;

		public static DamageInfo Create(int damage, Vector2 source, float knockbackForce = 80f)
		{
			return new DamageInfo
			{
				Damage = damage,
				SourcePosition = source,
				KnockbackForce = knockbackForce
			};
		}
	}

	private Entity owner;
	private Timer invulnerabilityTimer;
	private Area2D hurtbox;
	private Area2D hitbox;

	public override void _Ready()
	{
		try
		{
			owner = GetParent<Entity>();
			if(owner == null)
			{
				GD.PrintErr("CombatController: Owner (Entity) não encontrado!");
				return;
			}

			hurtbox = GetNodeOrNull<Area2D>("Hurtbox");
			hitbox = GetNodeOrNull<Area2D>("Hitbox");

			if(hurtbox == null)
				GD.PrintErr($"{owner.Name}: Hurtbox não encontrada!");
			if(hitbox == null)
				GD.PrintErr($"{owner.Name}: Hitbox não encontrada!");

			// Timer de invulnerabilidade
			invulnerabilityTimer = new Timer();
			AddChild(invulnerabilityTimer);
			invulnerabilityTimer.OneShot = true;
			invulnerabilityTimer.Timeout += () => {
				if(owner != null) owner.IsInvulnerable = false;
			};

			// Eventos de colisão
			if(hurtbox != null)
			{
				hurtbox.AreaEntered += OnHurtboxEntered;
				hurtbox.BodyEntered += OnHurtboxBodyEntered;
			}

			// Animação de ataque
			var sprite = owner.GetNodeOrNull<SpriteController>("SpriteController");
		// Frame changed não é mais necessário - hitbox gerenciada diretamente
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"CombatController: Erro na inicialização: {e.Message}");
		}
	}

	
	public int GetCurrentHealth() => owner?.Health ?? 0;

	/// <summary>
	/// Knockback simples NES: sempre afasta da fonte do dano
	/// </summary>
	private Vector2 CalculateSimpleKnockback(Vector2 sourcePos, float force)
	{
		try
		{
			if(owner == null) return Vector2.Zero;

			float horizontalDir = Mathf.Sign(owner.GlobalPosition.X - sourcePos.X);

			// Se muito próximos, usa direção que está olhando
			if(Mathf.Abs(horizontalDir) < 0.1f)
				horizontalDir = owner.FacingRight ? -1 : 1;

			// Knockback mais suave para enemies
			if(owner is Enemy)
				force *= 0.5f;

			return new Vector2(
				horizontalDir * force,
				-force * 0.3f  // Pouco impulso vertical
			);
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"CombatController: Erro em CalculateSimpleKnockback: {e.Message}");
			return Vector2.Zero;
		}
	}

	public void TakeDamage(DamageInfo damageInfo)
	{
		try
		{
			if(owner == null || owner.IsInvulnerable) return;

			owner.Health -= damageInfo.Damage;
			owner.IsInvulnerable = true;

			// Timer de invulnerabilidade
			if(owner.data != null)
			{
				invulnerabilityTimer.Start(owner.data.InvulnerabilityTime);
			}

			// Calcula knockback simples
			Vector2 knockbackDir = CalculateSimpleKnockback(damageInfo.SourcePosition, damageInfo.KnockbackForce);

			// Emite sinais
			EmitSignal(SignalName.Damaged, damageInfo.Damage, knockbackDir);

			if(owner.Health <= 0)
			{
				EmitSignal(SignalName.Died);
			}
		}
		catch(System.Exception e)
		{
			GD.PrintErr($"CombatController: Erro em TakeDamage: {e.Message}");
		}
	}
	
	
	public void SetHitboxActive(bool active, Vector2 offset, Vector2 size, int damage)
	{
		hitbox.CallDeferred("set_monitoring", active);
		hitbox.CallDeferred("set_position", offset);

		var shape = (CollisionShape2D)hitbox.GetChild(0);
		var newShape = new RectangleShape2D { Size = size };
		shape.CallDeferred("set_shape", newShape);

		hitbox.SetMeta("damage", damage);
		hitbox.SetMeta("source_pos", owner.GlobalPosition);

		// Adiciona ao grupo correto
		if(active)
		{
			string groupName = owner is Player ? "player_hitbox" : "enemy_hitbox";
			hitbox.CallDeferred("add_to_group", groupName);
		}
		else
		{
			hitbox.CallDeferred("remove_from_group", "player_hitbox");
			hitbox.CallDeferred("remove_from_group", "enemy_hitbox");
		}
	}
	
	private void OnHurtboxEntered(Area2D area)
	{
		if(area.IsInGroup("enemy_hitbox") || area.IsInGroup("player_hitbox"))
		{
			// Impede dano self-inflicted
			var sourcePos = (Vector2)area.GetMeta("source_pos");
			bool isSelfDamage = owner.GlobalPosition.DistanceTo(sourcePos) < 1.0f;

			if(!isSelfDamage)
			{
				int damage = (int)area.GetMeta("damage");
				var damageInfo = DamageInfo.Create(damage, sourcePos, owner.data.KnockbackForce);
				TakeDamage(damageInfo);
			}
		}
	}
	
	private void OnHurtboxBodyEntered(Node2D body)
	{
		// Apenas player toma dano por contato
		if(owner is not Player)
			return;

		if(body.IsInGroup("damage_on_contact"))
		{
			var contactEntity = body as Entity;
			if(contactEntity == null)
				return;

			var damageInfo = DamageInfo.Create(
				contactEntity.data.ContactDamage,
				body.GlobalPosition,
				contactEntity.data.KnockbackForce
			);

			TakeDamage(damageInfo);
		}
	}
}
