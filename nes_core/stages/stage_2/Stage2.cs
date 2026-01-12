using Godot;

/// <summary>
/// Implementação da Stage 2.
/// Fase intermediária com novos inimigos e mecânicas.
/// </summary>
public partial class Stage2 : Stage
{
	private Area2D bossDoor;
	private Node movingPlatforms;

	/// <summary>
	/// Configura elementos específicos da Stage 2.
	/// </summary>
	protected override void OnStageReady()
	{
		SetupBossDoor();
		SetupMovingPlatforms();
		SpawnStage2Enemies();
		LogStageInfo();
	}

	/// <summary>
	/// Configura a porta do boss.
	/// </summary>
	private void SetupBossDoor()
	{
		if (Data.HasBoss)
		{
			bossDoor = GetRequiredNode<Area2D>("BossDoor");
			bossDoor.BodyEntered += OnBossDoorEntered;
		}
	}

	/// <summary>
	/// Configura plataformas móveis.
	/// </summary>
	private void SetupMovingPlatforms()
	{
		movingPlatforms = GetNode<Node>("MovingPlatforms");
		if (movingPlatforms != null)
		{
			// Ativar movimento das plataformas
			foreach (Node child in movingPlatforms.GetChildren())
			{
				if (child is PathFollow2D platform)
				{
					// Configurar movimento da plataforma
				}
			}
		}
	}

	/// <summary>
	/// Spawna inimigos específicos da stage 2.
	/// </summary>
	private void SpawnStage2Enemies()
	{
		var enemiesContainer = GetNode<Node>("Enemies");
		if (enemiesContainer != null)
		{
			// Spawn de inimigos mais avançados
			GD.Print($"Stage 2: Spawnando {Data.MaxEnemies} inimigos com dificuldade {Data.EnemyDifficulty}");
		}
	}

	/// <summary>
	/// Log de informações específicas da stage.
	/// </summary>
	private void LogStageInfo()
	{
		GD.Print($"Stage 2 configurada - Boss: {Data.HasBoss}, Plataformas móveis ativas");
	}

	/// <summary>
	/// Chamado quando o jogador entra na área do boss.
	/// </summary>
	private void OnBossDoorEntered(Node2D body)
	{
		if (body is PlayerMain)
		{
			GD.Print("Boss fight iniciado!");
			// Transição para cena de boss ou spawn do boss
		}
	}
}