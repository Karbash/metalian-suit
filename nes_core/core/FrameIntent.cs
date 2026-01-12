using Godot;

/// <summary>
/// Representa a "intenção" calculada do frame atual.
/// Estados reagem a isso, não leem input diretamente.
/// </summary>

public struct FrameIntent
{
	// Movement
	public int MoveX;          // -1, 0, 1
	public int MoveY;          // -1, 0, 1 (para escadas/agachar)
	
	// Actions
	public bool JumpPressed;
	public bool JumpReleased;
	public bool AttackPressed;
	
	// Physics
	public bool OnFloor;
	public bool OnWall;
	public bool OnCeiling;
	
	// AI (para inimigos)
	public Vector2 TargetPosition;
	public float DistanceToTarget;
}
