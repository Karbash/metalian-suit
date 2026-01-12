using Godot;


[GlobalClass]
public partial class EntityData : Resource
{
	[ExportCategory("Stats")]
	[Export] public int MaxHealth = 3;
	
	[ExportCategory("Movement")]
	[Export] public float MoveSpeed = 80f;
	[Export] public float Gravity = 8f;
	[Export] public float MaxSpeed = 100f;
	[Export] public float MaxFallSpeed = 160f;
	[Export] public float JumpForce = -120f;
	
	[ExportCategory("Combat")]
	[Export] public int AttackDamage = 1;
	[Export] public int ContactDamage = 1;
	[Export] public int[] VulnerableTo = { (int)DamageType.Physical };
	
	[ExportCategory("Damage Response")]
	[Export] public float InvulnerabilityTime = 1.0f;
	[Export] public float KnockbackForce = 60f;
}
