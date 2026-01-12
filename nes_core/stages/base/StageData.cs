using Godot;

/// <summary>
/// Dados de configuração para uma stage.
/// Tudo configurável no editor - sem código hard-coded.
/// </summary>
[GlobalClass]
public partial class StageData : Resource
{
	[ExportCategory("Identificação")]
	[Export] public string StageName = "Stage 1";
	[Export] public string Description = "Descrição da stage";

	[ExportCategory("Visual")]
	[Export] public Texture2D Background;
	[Export] public Color AmbientColor = Colors.White;

	[ExportCategory("Áudio")]
	[Export] public AudioStream Music;
	[Export] public AudioStream AmbientSound;

	[ExportCategory("Gameplay")]
	[Export] public bool HasBoss = false;
	[Export] public bool HasCheckpoint = true;
	[Export] public int TimeLimit = 300; // segundos

	[ExportCategory("Dificuldade")]
	[Export] public float EnemyDifficulty = 1.0f;
	[Export] public int MaxEnemies = 10;

	[ExportCategory("Recompensas")]
	[Export] public int CompletionBonus = 5000;
	[Export] public int TimeBonus = 1000; // por segundo restante
	[Export] public int PerfectBonus = 10000; // sem dano

	/// <summary>
	/// Calcula o bônus total baseado no tempo restante.
	/// </summary>
	public int CalculateTimeBonus(float timeRemaining)
	{
		return Mathf.Max(0, (int)(timeRemaining * TimeBonus));
	}

	/// <summary>
	/// Verifica se a configuração é válida.
	/// </summary>
	public bool IsValid()
	{
		return !string.IsNullOrEmpty(StageName) &&
			   TimeLimit > 0 &&
			   CompletionBonus >= 0;
	}
}
