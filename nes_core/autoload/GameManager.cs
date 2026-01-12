using Godot;
using System.Collections.Generic;

/// <summary>
/// Gerenciador central do jogo.
/// Persiste durante toda a sessão, nunca é destruído.
/// Controla: vidas, score, stage atual, transições.
/// </summary>
public partial class GameManager : Node
{
	// Singleton
	public static GameManager Instance { get; private set; }
	
	// Estado do jogo
	public int Lives { get; private set; } = 3;
	public int Score { get; private set; } = 0;
	public int CurrentStageIndex { get; private set; } = 0;
	
	// Power-ups coletados (persiste entre stages)
	public HashSet<string> CollectedPowerUps = new();
	
	// Checkpoint atual (dentro da stage)
	public Vector2 CheckpointPosition { get; private set; } = Vector2.Zero;
	public bool HasCheckpoint { get; private set; } = false;
	
	// Managers
	private StageManager stageManager;
	private TransitionManager transitionManager;
	
	// Signals
	[Signal] public delegate void LivesChangedEventHandler(int lives);
	[Signal] public delegate void ScoreChangedEventHandler(int score);
	[Signal] public delegate void GameOverEventHandler();
	
	public override void _Ready()
	{
		if(Instance != null)
		{
			QueueFree();
			return;
		}
		
		Instance = this;
		ProcessMode = ProcessModeEnum.Always; // Não pausa
		
		stageManager = new StageManager();
		AddChild(stageManager);
		
		transitionManager = new TransitionManager();
		AddChild(transitionManager);
		
		GD.Print("=== GAME MANAGER INITIALIZED ===");
	}
	
	// ==================== GAME FLOW ====================
	
	public void StartNewGame()
	{
		Lives = 3;
		Score = 0;
		CurrentStageIndex = 0;
		CollectedPowerUps.Clear();
		ClearCheckpoint();
		
		EmitSignal(SignalName.LivesChanged, Lives);
		EmitSignal(SignalName.ScoreChanged, Score);
		
		LoadStage(0);
	}
	
	public void ContinueGame()
	{
		// Continue = mantém score/power-ups, reseta vidas
		Lives = 3;
		EmitSignal(SignalName.LivesChanged, Lives);
		
		LoadStage(CurrentStageIndex);
	}
	
	public void LoadStage(int stageIndex)
	{
		CurrentStageIndex = stageIndex;
		ClearCheckpoint();
		
		transitionManager.FadeOut(() => 
		{
			stageManager.LoadStage(stageIndex);
			transitionManager.FadeIn();
		});
	}
	
	public void LoadNextStage()
	{
		LoadStage(CurrentStageIndex + 1);
	}
	
	public void ReloadCurrentStage()
	{
		LoadStage(CurrentStageIndex);
	}
	
	// ==================== PLAYER STATE ====================
	
	public void OnPlayerDied()
	{
		Lives--;
		EmitSignal(SignalName.LivesChanged, Lives);
		
		if(Lives <= 0)
		{
			// Game Over
			EmitSignal(SignalName.GameOver);
			transitionManager.FadeOut(() =>
			{
				stageManager.UnloadCurrentStage();
				// Mostrar tela de Game Over
			});
		}
		else
		{
			// Respawn
			if(HasCheckpoint)
				RespawnAtCheckpoint();
			else
				ReloadCurrentStage();
		}
	}
	
	public void AddScore(int points)
	{
		Score += points;
		EmitSignal(SignalName.ScoreChanged, Score);
		
		// Extra life a cada 10000 pontos (estilo NES)
		if(Score % 10000 < points)
		{
			AddLife();
		}
	}
	
	public void AddLife()
	{
		Lives++;
		EmitSignal(SignalName.LivesChanged, Lives);
		// TODO: Som de extra life
	}
	
	// ==================== CHECKPOINT ====================
	
	public void SetCheckpoint(Vector2 position)
	{
		CheckpointPosition = position;
		HasCheckpoint = true;
		GD.Print($"Checkpoint set at {position}");
	}
	
	public void ClearCheckpoint()
	{
		HasCheckpoint = false;
		CheckpointPosition = Vector2.Zero;
	}
	
	private void RespawnAtCheckpoint()
	{
		transitionManager.QuickFade(() => 
		{
			var player = GetTree().GetFirstNodeInGroup("player") as PlayerMain;
			if(player != null)
			{
				player.GlobalPosition = CheckpointPosition;
				player.Respawn();
			}
		});
	}
	
	// ==================== POWER-UPS ====================
	
	public void CollectPowerUp(string powerUpId)
	{
		CollectedPowerUps.Add(powerUpId);
		GD.Print($"Power-up coletado: {powerUpId}");
	}
	
	public bool HasPowerUp(string powerUpId)
	{
		return CollectedPowerUps.Contains(powerUpId);
	}
	
	// ==================== PAUSE ====================
	
	public void TogglePause()
	{
		GetTree().Paused = !GetTree().Paused;
	}
}
