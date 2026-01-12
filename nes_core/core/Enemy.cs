using Godot;

public abstract partial class Enemy : Entity
{
	[Export] public BrainDropData BrainDrop;

	protected Player player;
	private string previousState = "patrol";
	
	public override void _Ready()
	{
		base._Ready();

		// Adiciona ao grupo de damage on contact
		AddToGroup("damage_on_contact");

		// Busca player (lazy load)
		CallDeferred(nameof(FindPlayer));
	}

	public string GetPreviousState() => previousState;

	public void SetPreviousState(string state) => previousState = state;
	
	private void FindPlayer()
	{
		player = GetTree().GetFirstNodeInGroup("player") as Player;
	}
	
	protected override FrameIntent BuildIntent()
	{
		var intent = new FrameIntent
		{
			MoveX = GetAIMove(),
			OnFloor = IsOnFloor(),
			OnWall = IsOnWall()
		};
		
		if(player != null)
		{
			intent.TargetPosition = player.GlobalPosition;
			intent.DistanceToTarget = GlobalPosition.DistanceTo(player.GlobalPosition);
		}
		
		return intent;
	}
	
	/// <summary>
	/// Override em cada inimigo para definir AI.
	/// Walker: retorna direção fixa
	/// Jumper: retorna 0 (não anda)
	/// Turret: retorna 0 (não se move)
	/// Brain: retorna direção para player
	/// </summary>
	protected abstract int GetAIMove();
	
	protected override void OnDied()
	{
		base.OnDied();
		
		// Drop brain
		if(BrainDrop != null && GD.Randf() <= BrainDrop.DropChance)
		{
			BrainDrop.Spawn(GlobalPosition);
		}
	}
}
