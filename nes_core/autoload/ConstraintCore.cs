using Godot;

/// <summary>
/// Autoload que define regras imutáveis de design NES.
/// Qualquer violação dessas flags é bug de design.
/// </summary>
public partial class ConstraintCore : Node
{
	// Constantes NES
	public const int NES_FPS = 60;
	public const float FIXED_DELTA = 1f / NES_FPS;
	
	// Regras de design (IMUTÁVEIS)
	public static bool AllowCancel = false;          // Ataques não cancelam
	public static bool AllowAirControl = false;      // Sem controle fino no ar
	public static bool AllowAttackCancel = false;    // Ataque trava até Recovery
	
	// Debug
	[Export] public bool ShowHitboxes = false;
	
	public override void _Ready()
	{
		GD.Print("=== CONSTRAINT CORE LOADED ===");
		GD.Print($"- Attack Cancel: {AllowAttackCancel}");
		GD.Print($"- Air Control: {AllowAirControl}");
		GD.Print($"- Cancel States: {AllowCancel}");
	}
}
