using Godot;

public abstract partial class Player : Entity
{
	protected override FrameIntent BuildIntent()
	{
		var moveX = input.GetHorizontalAxis();
		var facingBefore = FacingRight;

		var intent = new FrameIntent
		{
			MoveX = moveX,
			MoveY = input.GetVerticalAxis(),
			JumpPressed = input.IsJustPressed(InputController.NESButton.A),
			JumpReleased = input.IsJustReleased(InputController.NESButton.A),
			AttackPressed = input.IsJustPressed(InputController.NESButton.B),
			OnFloor = IsOnFloor(),
			OnWall = IsOnWall(),
			OnCeiling = IsOnCeiling()
		};

		// Log quando direção muda
		if(moveX != 0)
		{
			var wouldFaceRight = moveX > 0;
			if(facingBefore != wouldFaceRight)
			{
				GD.Print($"Input flip: MoveX={moveX}, would face {wouldFaceRight}");
			}
		}

		return intent;
	}
	
	public void Freeze()
	{
		physicsController.StopX();
		SetProcess(false);
		SetPhysicsProcess(false);
	}
	
	public void Respawn()
	{
		SetProcess(true);
		SetPhysicsProcess(true);
		physicsController.StopX();
		stateMachine.ChangeState("idle");
	}
}
