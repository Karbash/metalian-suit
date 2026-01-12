using Godot;

public class PlayerHurtState : State
{
	protected override string AnimationName => null; // Não toca animação específica, deixa a atual

	public PlayerHurtState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();

		// Knockback é aplicado automaticamente pelo KnockbackController
		// Estado apenas aguarda fim
	}

	public override void Update(double delta)
	{
		base.Update(delta);

		var knockback = entity.GetNodeOrNull<KnockbackController>("KnockbackController");

		// Aguarda fim do knockback (ou usa timer fallback se não houver controller)
		bool canReturnToNormal = knockback == null || !knockback.IsInKnockback;

		if(canReturnToNormal)
		{
			// Retorna ao estado apropriado
			if(intent.OnFloor)
			{
				ChangeState("idle");
			}
			else
			{
				ChangeState("air");
			}
		}
	}
}
