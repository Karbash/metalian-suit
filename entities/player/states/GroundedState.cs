using Godot;

/// <summary>
/// Estado base para Idle e Walk.
/// Centraliza lógica comum de chão.
/// </summary>

public abstract class GroundedState : State
{
	public GroundedState(Entity entity) : base(entity) { }
	
	public override void Update(double delta)
	{
		base.Update(delta);

		// Caiu do chão
		if(!intent.OnFloor)
		{
			ChangeState("air");
			return;
		}

		// Ataque
		if(intent.AttackPressed)
		{
			ChangeState("attack");
			return;
		}

		// Pulo
		if(intent.JumpPressed)
		{
			ChangeState("air");
			physics.Jump(entity.data.JumpForce);
			return;
		}
		
		// Movimento horizontal
		if(intent.MoveX != 0)
		{
			var newFacingRight = intent.MoveX > 0;
			if(entity.FacingRight != newFacingRight)
			{
				GD.Print($"Player flip: {entity.FacingRight} -> {newFacingRight}, MoveX: {intent.MoveX}");
			}
			entity.FacingRight = newFacingRight;
			physics.Move(intent.MoveX);
			
			// Transição Idle → Walk
			if(this is PlayerIdleState)
			{
				ChangeState("walk");
				return;
			}
		}
		else
		{
			physics.StopX();
			
			// Transição Walk → Idle
			if(this is PlayerWalkState)
			{
				ChangeState("idle");
				return;
			}
		}
	}
}
