using Godot;

/// <summary>
/// Jumper NES-style: Fica parado, prepara pulo, pula alto.
/// Comportamento simples e previsível.
/// </summary>
public partial class Jumper : Enemy
{
	private float jumpTimer;
	private bool isJumping;
	private float telegraphTime = 1.0f; // Tempo de preparação
	private float idleTime = 2.0f; // Tempo parado

	protected override void UpdateAI(double delta)
	{
		jumpTimer -= (float)delta;

		if(isJumping)
		{
			// Durante pulo, não faz nada
			if(IsOnFloor())
			{
				isJumping = false;
				jumpTimer = idleTime; // Tempo de descanso
			}
		}
		else
		{
			// Preparação para pulo
			if(jumpTimer <= 0)
			{
				Jump();
			}
		}
	}

	private void Jump()
	{
		isJumping = true;
		physicsController.Jump(data.JumpForce * 1.5f); // Pulo mais alto
		jumpTimer = telegraphTime; // Próximo pulo
	}
}
