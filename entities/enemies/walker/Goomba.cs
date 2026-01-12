using Godot;

public partial class Goomba : Enemy
{
	private float direction = -1f;
	
	protected override int GetAIMove()
	{
		return (int)direction;
	}
	
	protected override void SetupStateMachine()
	{
		stateMachine.AddState("patrol", new WalkerPatrolState(this));
		stateMachine.AddState("hurt", new EnemyHurtState(this));
		stateMachine.AddState("dead", new WalkerDeadState(this));

		stateMachine.ChangeState("patrol");
	}
	
	public void FlipDirection()
	{
		direction *= -1;
		FacingRight = direction > 0;

		// ✅ FORÇA ATUALIZAÇÃO DO SPRITE QUANDO MUDA DIREÇÃO
		// Reproduz a animação atual com o novo facing
		if(spriteController != null)
		{
			// Força re-execução da animação atual com novo facing
			var currentAnim = spriteController.GetCurrentAnimation();
			if(!string.IsNullOrEmpty(currentAnim))
			{
				spriteController.Play(currentAnim, FacingRight, true);
			}
		}
	}
}
