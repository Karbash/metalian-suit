using Godot;

/// <summary>
/// Estado aéreo unificado.
/// Animação muda automaticamente baseado em velocidade Y.
/// </summary>

public class PlayerAirState : State
{
	protected override string AnimationName 
		=> physics.Velocity.Y < 0 ? "jump" : "fall";
	
	public PlayerAirState(Entity entity) : base(entity) { }
	
	public override void Enter()
	{
		base.Enter();
	}
	
	public override void Update(double delta)
	{
		base.Update(delta);
		
		// Controle aéreo (limitado se ConstraintCore.AllowAirControl = false)
		if(intent.MoveX != 0)
		{
			entity.FacingRight = intent.MoveX > 0;

			if(ConstraintCore.AllowAirControl)
			{
				physics.Move(intent.MoveX);
			}
			else
			{
				// NES: movimento aéreo é mais "deslizante"
				physics.Velocity = new Vector2(
					intent.MoveX * entity.data.MoveSpeed * 0.7f,
					physics.Velocity.Y
				);
			}
		}
		
		// Pulo variável (soltar botão = cai mais rápido)
		if(intent.JumpReleased && physics.Velocity.Y < 0)
		{
			physics.CutJump();
		}
		
		// Ataque aéreo
		if(intent.AttackPressed)
		{
			ChangeState("air_attack");
			return;
		}
		
		// Pousou
		if(intent.OnFloor)
		{
			ChangeState(intent.MoveX != 0 ? "walk" : "idle");
			return;
		}
		
		// Atualiza animação se mudou direção vertical
		PlayAnimation();
	}
}
