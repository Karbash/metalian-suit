using Godot;

/// <summary>
/// Implementação da Stage 1.
/// Fase introdutória com mecânicas básicas.
/// </summary>
public partial class Stage1 : Stage
{
	private Area2D goalArea;
	private Checkpoint checkpoint;

	public override void _Ready()
	{
		// Garante que o Data está definido antes da inicialização
		EnsureDataIsSet();
		base._Ready();
	}

	/// <summary>
	/// Configura elementos específicos da Stage 1.
	/// </summary>
	protected override void OnStageReady()
	{
		SetupGoalArea();
		SetupCheckpoint();
		SpawnStageEnemies();
		LogStageInfo();
	}

	/// <summary>
	/// Garante que o StageData está corretamente definido.
	/// Como Stage1 instancia StageBase, precisamos garantir que o Data seja passado.
	/// </summary>
	private void EnsureDataIsSet()
	{
		// O Data já deve estar definido via export na cena
		// Mas vamos garantir que ele seja válido
		if (Data == null)
		{
			GD.PrintErr("StageData não definido no Stage1! Carregando dados padrão...");
			var dataPath = "res://nes_core/stages/stage_1/stage_1_data.tres";
			Data = GD.Load<StageData>(dataPath);
			if (Data == null)
			{
				GD.PrintErr("Falha ao carregar StageData! Criando dados padrão...");
				Data = new StageData
				{
					StageName = "Stage 1",
					TimeLimit = 300,
					CompletionBonus = 5000
				};
			}
		}
	}

	/// <summary>
	/// Configura a área de chegada da stage.
	/// </summary>
	private void SetupGoalArea()
	{
		goalArea = GetRequiredNode<Area2D>("GoalArea");
		goalArea.BodyEntered += OnGoalReached;
	}

	/// <summary>
	/// Configura o checkpoint se disponível.
	/// </summary>
	private void SetupCheckpoint()
	{
		if (Data.HasCheckpoint)
		{
			checkpoint = GetRequiredNode<Checkpoint>("Checkpoint");
			checkpoint.Activated += OnCheckpointActivated;
		}
	}

	/// <summary>
	/// Spawna inimigos específicos da stage 1.
	/// </summary>
	private void SpawnStageEnemies()
	{
		// Spawn Goomba no container Enemies
		var enemiesContainer = GetNode<Node>("Enemies");
		if (enemiesContainer != null)
		{
			// Inimigos já foram adicionados na cena, mas poderiam ser spawnados dinamicamente aqui
			GD.Print($"Stage 1: {enemiesContainer.GetChildCount()} inimigos prontos");
		}
	}

	/// <summary>
	/// Log de informações específicas da stage.
	/// </summary>
	private void LogStageInfo()
	{
		GD.Print($"Stage 1 configurada - Dificuldade: {Data.EnemyDifficulty}, Tempo: {Data.TimeLimit}s");
	}

	/// <summary>
	/// Chamado quando o jogador alcança a área de chegada.
	/// </summary>
	private void OnGoalReached(Node2D body)
	{
		if (body is PlayerMain player)
		{
			GD.Print("Jogador chegou ao fim da Stage 1!");
			player.Freeze();
			CompleteStage();
		}
	}

	/// <summary>
	/// Chamado quando um checkpoint é ativado.
	/// </summary>
	private void OnCheckpointActivated(Vector2 position)
	{
		GD.Print($"Checkpoint ativado na posição: {position}");
		GameManager.Instance.SetCheckpoint(position);
	}
}
