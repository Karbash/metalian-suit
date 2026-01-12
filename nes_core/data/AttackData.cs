using Godot;
using Godot.Collections;

[GlobalClass]
public partial class HitboxFrameData : Resource
{
	[Export] public int Frame;
	[Export] public Vector2 Offset;
	[Export] public Vector2 Size;
}

[GlobalClass]
public partial class AttackData : Resource
{
	[Export] public string AttackName;
	[Export] public int Damage = 1;
	[Export] public Array<HitboxFrameData> HitboxFrames;
	
	[ExportGroup("Timing (frames)")]
	[Export] public int StartupFrames = 3;
	[Export] public int ActiveFrames = 5;
	[Export] public int RecoveryFrames = 8;
	
	public int TotalFrames => StartupFrames + ActiveFrames + RecoveryFrames;
}
