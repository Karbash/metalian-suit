using Godot;

/// <summary>
/// Turret NES-style: Atira projéteis em intervalo fixo.
/// Comportamento simples: espera → atira → recarrega.
/// </summary>
public partial class Turret : Enemy
{
	[Export] public PackedScene ProjectileScene;

	private float fireTimer;
	private float fireInterval = 3.0f; // Atira a cada 3 segundos

	protected override void UpdateAI(double delta)
	{
		fireTimer -= (float)delta;

		if(fireTimer <= 0)
		{
			FireProjectile();
			fireTimer = fireInterval;
		}
	}

	private void FireProjectile()
	{
		if(ProjectileScene == null) return;

		var projectile = ProjectileScene.Instantiate<Node2D>();
		projectile.GlobalPosition = GlobalPosition + new Vector2(FacingRight ? 10 : -10, 0);
		GetTree().Root.AddChild(projectile);

		// Configurar direção do projétil
		if(projectile is Projectile proj)
		{
			proj.SetDirection(FacingRight ? 1 : -1);
		}
	}
}
