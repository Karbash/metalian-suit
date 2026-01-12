using Godot;

/// <summary>
/// Tipos de knockback estilo NES.
/// Cada tipo tem comportamento fixo e previsível.
/// </summary>
public enum KnockbackType
{
    None,           // Sem knockback (boss imune)
    Light,          // Pequeno empurrão (Metroid)
    Standard,       // Padrão (Mega Man)
    Heavy,          // Forte (Castlevania)
    Stun,           // Paralisa mas não empurra
    Launch          // Lança longe (Ninja Gaiden)
}

[GlobalClass]
public partial class KnockbackData : Resource
{
    [Export] public KnockbackType Type = KnockbackType.Standard;

    [ExportGroup("Force")]
    [Export] public float HorizontalForce = 80f;
    [Export] public float VerticalForce = -60f;

    [ExportGroup("Duration (frames a 60fps)")]
    [Export] public int StunFrames = 18;        // Sem controle
    [Export] public int InvulnerableFrames = 60; // I-frames

    [ExportGroup("Behavior")]
    [Export] public bool AllowAirControl = false;
    [Export] public bool AllowAttack = false;
    [Export] public bool CancelVelocity = true;  // Zera velocidade antes
    [Export] public bool RotateSprite = false;   // Gira sprite (Ninja Gaiden)

    // Presets NES
    public static KnockbackData MegaMan => new()
    {
        Type = KnockbackType.Standard,
        HorizontalForce = 80f,
        VerticalForce = -40f,
        StunFrames = 18,
        InvulnerableFrames = 60,
        AllowAirControl = false,
        AllowAttack = false,
        CancelVelocity = true
    };

    public static KnockbackData Castlevania => new()
    {
        Type = KnockbackType.Heavy,
        HorizontalForce = 120f,
        VerticalForce = -100f,
        StunFrames = 30,
        InvulnerableFrames = 90,
        AllowAirControl = false,
        AllowAttack = false,
        CancelVelocity = true
    };

    public static KnockbackData Metroid => new()
    {
        Type = KnockbackType.Light,
        HorizontalForce = 60f,
        VerticalForce = 0f,
        StunFrames = 10,
        InvulnerableFrames = 40,
        AllowAirControl = true,
        AllowAttack = true,
        CancelVelocity = false
    };

    public static KnockbackData NinjaGaiden => new()
    {
        Type = KnockbackType.Launch,
        HorizontalForce = 150f,
        VerticalForce = -120f,
        StunFrames = 40,
        InvulnerableFrames = 60,
        AllowAirControl = false,
        AllowAttack = false,
        CancelVelocity = true,
        RotateSprite = true
    };

    public static KnockbackData BossStagger => new()
    {
        Type = KnockbackType.Stun,
        HorizontalForce = 0f,
        VerticalForce = 0f,
        StunFrames = 15,
        InvulnerableFrames = 30,
        AllowAirControl = false,
        AllowAttack = false,
        CancelVelocity = false
    };
}