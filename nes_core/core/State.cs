using Godot;

/// <summary>
/// Estado base. Não opina sobre input ou física.
/// Apenas reage ao FrameIntent fornecido pela Entity.
/// </summary>

public abstract class State
{
	protected Entity entity;
	protected SpriteController sprite;
	protected PhysicsController physics;
	protected CombatController combat;

	public Entity Entity => entity; // Propriedade pública para acesso
	
	protected FrameIntent intent;
	
	protected abstract string AnimationName { get; }
	
	public State(Entity entity)
	{
		this.entity = entity;
		this.sprite = entity.GetNode<SpriteController>("SpriteController");
		this.physics = entity.physicsController;
		this.combat = entity.GetNode<CombatController>("CombatController");
	}
	
	public virtual void Enter()
	{
		PlayAnimation();
	}
	
	public virtual void Update(double delta)
	{
		intent = entity.GetIntent();
	}
	
	public virtual void Exit() { }
	
	protected void PlayAnimation(bool forceRestart = false)
	{
		if(AnimationName != null)
			sprite.Play(AnimationName, entity.FacingRight, forceRestart);
	}
	
	protected void ChangeState(string name)
	{
		entity.stateMachine.ChangeState(name);
	}
}
