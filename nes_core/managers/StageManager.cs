using Godot;
using System.Collections.Generic;

/// <summary>
/// Gerencia o ciclo de vida das stages.
/// Responsável por carregar, descarregar e transitar entre stages.
/// </summary>
public partial class StageManager : Node
{
	private Stage currentStage;
	private Node stageContainer;

	/// <summary>
	/// Mapeamento de índices para caminhos das stages.
	/// Facilita adição/remoção de stages sem alterar código.
	/// </summary>
	private readonly Dictionary<int, string> stagePaths = new()
	{
		[0] = "res://nes_core/stages/stage_1/Stage1.tscn",
		[1] = "res://nes_core/stages/stage_2/Stage2.tscn",
		// Adicione novas stages aqui seguindo o padrão
	};

	public override void _Ready()
	{
		// Não tenta encontrar o container imediatamente, pois pode não existir ainda
		// (o jogo pode estar na TitleScreen)
	}

	/// <summary>
	/// Localiza o container onde as stages serão instanciadas.
	/// </summary>
	private void FindStageContainer()
	{
		var root = GetTree().Root;

		// Primeiro tenta encontrar diretamente
		stageContainer = root.GetNodeOrNull("Game/StageContainer");

		if (stageContainer == null)
		{
			// Fallback: busca recursiva por nome
			stageContainer = root.FindChild("StageContainer", true, false);
		}

		if (stageContainer == null)
		{
			// Último recurso: busca por tipo Node2D com nome StageContainer
			foreach (var child in root.GetChildren())
			{
				if (child is Node2D node2d && node2d.Name == "StageContainer")
				{
					stageContainer = node2d;
					break;
				}
			}
		}

		if (stageContainer == null)
		{
			GD.PrintErr("StageContainer não encontrado! Verifique a cena Game.tscn");
			GD.Print($"Cena raiz atual: {root.SceneFilePath}");

			// Monta lista de nomes dos filhos sem usar LINQ
			var childNames = new System.Collections.Generic.List<string>();
			foreach (var child in root.GetChildren())
			{
				childNames.Add(child.Name);
			}
			GD.Print($"Filhos da raiz: {string.Join(", ", childNames)}");
		}
		else
		{
			GD.Print("StageContainer encontrado com sucesso");
		}
	}
	
	/// <summary>
	/// Carrega uma stage pelo índice.
	/// </summary>
	public void LoadStage(int stageIndex)
	{
		if (!ValidateStageIndex(stageIndex)) return;
		if (!EnsureStageContainer()) return;

		UnloadCurrentStage();

		var stagePath = stagePaths[stageIndex];
		var stageScene = GD.Load<PackedScene>(stagePath);

		if (stageScene == null)
		{
			GD.PrintErr($"Falha ao carregar cena: {stagePath}");
			return;
		}

		var node = stageScene.Instantiate();
		currentStage = node as Stage;

		if (currentStage == null)
		{
			GD.PrintErr($"Cena {stagePath} não herda de Stage");
			LogDebugInfo(node, stagePath);
			node.QueueFree();
			return;
		}

		stageContainer.AddChild(currentStage);
		ConnectStageSignals();

		GD.Print($"Stage {stageIndex} carregada: {currentStage.Data?.StageName ?? "Unknown"}");
	}

	/// <summary>
	/// Valida se o índice da stage é válido.
	/// </summary>
	private bool ValidateStageIndex(int index)
	{
		if (index < 0 || index >= stagePaths.Count)
		{
			GD.PrintErr($"Índice de stage inválido: {index}");
			return false;
		}
		return true;
	}

	/// <summary>
	/// Garante que o container de stages está disponível.
	/// </summary>
	private bool EnsureStageContainer()
	{
		if (stageContainer == null)
		{
			FindStageContainer();
		}

		if (stageContainer == null)
		{
			GD.PrintErr("StageContainer não encontrado");
			return false;
		}
		return true;
	}

	/// <summary>
	/// Conecta os sinais da stage atual.
	/// </summary>
	private void ConnectStageSignals()
	{
		if (currentStage != null)
		{
			currentStage.StageCompleted += OnStageCompleted;
			currentStage.BossDefeated += OnBossDefeated;
		}
	}

	/// <summary>
	/// Log de informações de debug para falhas de carregamento.
	/// </summary>
	private void LogDebugInfo(Node node, string stagePath)
	{
		if (node == null)
		{
			GD.Print("Node é null - não foi possível instanciar a cena");
			return;
		}

		GD.Print($"Tipo do nó: {node.GetType()}");
		var script = node.GetScript();
		var scriptObj = script.As<Script>();
		if (scriptObj != null)
		{
			GD.Print($"Script: {scriptObj.ResourcePath}");
		}
		else
		{
			GD.Print("Script não encontrado no nó");
		}
	}
	
	/// <summary>
	/// Descarrega a stage atual.
	/// </summary>
	public void UnloadCurrentStage()
	{
		if (currentStage != null)
		{
			DisconnectStageSignals();
			currentStage.QueueFree();
			currentStage = null;
		}
	}

	/// <summary>
	/// Desconecta os sinais da stage atual.
	/// </summary>
	private void DisconnectStageSignals()
	{
		if (currentStage != null)
		{
			currentStage.StageCompleted -= OnStageCompleted;
			currentStage.BossDefeated -= OnBossDefeated;
		}
	}
	
	/// <summary>
	/// Retorna a stage atualmente carregada.
	/// </summary>
	public Stage GetCurrentStage()
	{
		return currentStage;
	}

	// ==================== EVENT HANDLERS ====================

	/// <summary>
	/// Chamado quando uma stage é completada.
	/// </summary>
	private void OnStageCompleted()
	{
		GD.Print($"Stage '{currentStage?.Data?.StageName ?? "Unknown"}' completada!");
		GameManager.Instance.LoadNextStage();
	}

	/// <summary>
	/// Chamado quando um boss é derrotado.
	/// </summary>
	private void OnBossDefeated()
	{
		GD.Print("Boss derrotado! Preparando próxima stage...");
		// Delay para animação de vitória
		GetTree().CreateTimer(3.0).Timeout += () =>
		{
			GameManager.Instance.LoadNextStage();
		};
	}
}
