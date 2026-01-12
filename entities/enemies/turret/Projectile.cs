using Godot;

public partial class Projectile : Area2D
{
	[Export] public float Speed = 120f;
	[Export] public int Damage = 1;
	
	private Vector2 direction;
	
	public override void _Ready()
	{
		AddToGroup("projectile");
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
	}
	
	public void SetDirection(float dir)
	{
		direction = new Vector2(dir, 0);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += direction * Speed * (float)delta;
		
		// Remove se saiu da tela
		if(!GetViewport().GetVisibleRect().HasPoint(GlobalPosition))
		{
			QueueFree();
		}
	}
	
	private void OnBodyEntered(Node2D body)
	{
		// Acertou parede ou player
		if(body is Player player)
		{
			var combat = player.GetNode<CombatController>("CombatController");
			combat.TakeDamage(new CombatController.DamageInfo
			{
				Damage = Damage,
				SourcePosition = GlobalPosition,
				KnockbackForce = 60f
			});
		}
		
		QueueFree();
	}
	
	private void OnAreaEntered(Area2D area)
	{
		// Acertou hitbox do player
		QueueFree();
	}
}
