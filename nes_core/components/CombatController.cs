using Godot;
using System.Collections.Generic;

public partial class CombatController : Node2D
{
	[Signal] public delegate void DamagedEventHandler(int damage, Vector2 knockbackDir);
	[Signal] public delegate void DiedEventHandler();
	
	public struct DamageInfo
	{
		public int Damage;
		public DamageType Type;
		public Vector2 SourcePosition;

		// NOVO: Permite override de knockback por ataque
		public KnockbackData KnockbackOverride;

		// Factory methods
		public static DamageInfo Standard(int damage, Vector2 source)
		{
			return new DamageInfo
			{
				Damage = damage,
				Type = DamageType.Physical,
				SourcePosition = source,
				KnockbackOverride = null // Usa default
			};
		}

		public static DamageInfo WithKnockback(int damage, Vector2 source, KnockbackData knockback)
		{
			return new DamageInfo
			{
				Damage = damage,
				Type = DamageType.Physical,
				SourcePosition = source,
				KnockbackOverride = knockback
			};
		}
	}
	
	private Entity owner;
	private int health;
	private bool invulnerable;
	private Timer invulnerabilityTimer;
	private KnockbackController knockbackController;

	private Area2D hurtbox;
	private Area2D hitbox;
	
	// Hitbox configuration por attack
	private Dictionary<string, AttackData> attacks = new();
	
	public override void _Ready()
	{
		owner = GetParent<Entity>();
		health = owner.data.MaxHealth;

		hurtbox = GetNode<Area2D>("Hurtbox");
		hitbox = GetNode<Area2D>("Hitbox");

		// NOVO: Busca KnockbackController
		knockbackController = owner.GetNodeOrNull<KnockbackController>("KnockbackController");

		if(knockbackController == null)
		{
			GD.PrintErr($"{owner.Name} não tem KnockbackController!");
		}

		invulnerabilityTimer = new Timer();
		AddChild(invulnerabilityTimer);
		invulnerabilityTimer.OneShot = true;
		invulnerabilityTimer.Timeout += () => invulnerable = false;

		hurtbox.AreaEntered += OnHurtboxEntered;
		hurtbox.BodyEntered += OnHurtboxBodyEntered;

		// Escuta mudanças de frame para ativar hitboxes
		var sprite = owner.GetNode<SpriteController>("SpriteController");
		sprite.FrameChanged += OnAnimationFrameChanged;
	}
	
	public void RegisterAttack(string name, AttackData attackData)
	{
		attacks[name] = attackData;
	}
	
	private void OnAnimationFrameChanged(int frame)
	{
		// Hitbox agora é gerenciada diretamente pelos estados de ataque
		// Este método só desativa hitbox por segurança
		SetHitboxActive(false, Vector2.Zero, Vector2.Zero, 0);
	}
	
	public bool IsInvulnerable() => invulnerable;

	public int GetCurrentHealth() => health;

	public void TakeDamage(DamageInfo damageInfo, int? attackDirection = null)
	{
		// NOVO: Checa invulnerabilidade do knockback
		if(knockbackController != null && knockbackController.IsInvulnerable)
		{
			return;
		}

		if(invulnerable) return;
		if(!System.Array.Exists(owner.data.VulnerableTo, t => (DamageType)t == damageInfo.Type)) return;

		health -= damageInfo.Damage;
		invulnerable = true;
		invulnerabilityTimer.Start(owner.data.InvulnerabilityTime);

		// NOVO: Aplica knockback via controller
		if(knockbackController != null)
		{
			knockbackController.ApplyKnockback(
				damageInfo.SourcePosition,
				damageInfo.KnockbackOverride
			);
		}

		// Signal de dano (sem knockback, isso é feito pelo KnockbackController)
		EmitSignal(SignalName.Damaged, damageInfo.Damage, Vector2.Zero);

		if(health <= 0)
		{
			EmitSignal(SignalName.Died);
		}
	}
	
	
	public void SetHitboxActive(bool active, Vector2 offset, Vector2 size, int damage)
	{
		// Usa CallDeferred para mudanças de physics state durante physics callbacks
		hitbox.CallDeferred("set_monitoring", active);
		hitbox.CallDeferred("set_position", offset);

		var shape = (CollisionShape2D)hitbox.GetChild(0);
		var newShape = new RectangleShape2D { Size = size };
		shape.CallDeferred("set_shape", newShape);

		hitbox.SetMeta("damage", damage);
		hitbox.SetMeta("source_pos", owner.GlobalPosition);
		// ✅ DIREÇÃO DO ATAQUE INVERTIDA PARA AFASTAR O ALVO
		hitbox.SetMeta("attack_dir", owner.FacingRight ? -1 : 1);

		// Adiciona hitbox ao grupo correto baseado no tipo de entidade
		if(active)
		{
			if(owner is Player)
				hitbox.CallDeferred("add_to_group", "player_hitbox");
			else if(owner is Enemy)
				hitbox.CallDeferred("add_to_group", "enemy_hitbox");
		}
		else
		{
			// Remove de ambos os grupos quando desativa
			hitbox.CallDeferred("remove_from_group", "player_hitbox");
			hitbox.CallDeferred("remove_from_group", "enemy_hitbox");
		}
	}
	
	private void OnHurtboxEntered(Area2D area)
	{
		if(area.IsInGroup("enemy_hitbox") || area.IsInGroup("player_hitbox"))
		{
			// Impede dano self-inflicted: player não toma dano da própria hitbox
			var sourcePos = (Vector2)area.GetMeta("source_pos");
			bool isSelfDamage = owner.GlobalPosition.DistanceTo(sourcePos) < 1.0f; // Distância muito pequena = mesma entidade

			if(!isSelfDamage)
			{
				// ✅ RECUPERA DIREÇÃO DO ATAQUE DOS METADADOS
				int? attackDirection = null;
				if(area.HasMeta("attack_dir"))
				{
					attackDirection = (int)area.GetMeta("attack_dir");
				}

				var damageInfo = new DamageInfo
				{
					Damage = (int)area.GetMeta("damage"),
					Type = DamageType.Physical,
					SourcePosition = sourcePos
				};

				TakeDamage(damageInfo, attackDirection);
			}
		}
	}
	
	private void OnHurtboxBodyEntered(Node2D body)
	{
		// ✅ APENAS PLAYER DEVE TOMAR DANO POR CONTATO
		// Enemies não tomam dano quando o Player colide com eles
		if(owner is not Player)
			return;

		if(body.IsInGroup("damage_on_contact"))
		{
			var contactEntity = body as Entity;
			if(contactEntity == null)
				return;

			var damageInfo = new DamageInfo
			{
				Damage = contactEntity.data.ContactDamage,
				Type = DamageType.Physical,
				SourcePosition = body.GlobalPosition
			};

			TakeDamage(damageInfo);
		}
	}
}
