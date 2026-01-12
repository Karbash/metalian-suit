using Godot;
using System;

/// <summary>
/// Classe base para todas as stages.
/// Fornece infraestrutura comum e ciclo de vida padronizado.
/// </summary>
public abstract partial class Stage : Node2D
{
	[Export] public StageData Data;

	[Signal] public delegate void StageCompletedEventHandler();
	[Signal] public delegate void BossDefeatedEventHandler();

	protected PlayerMain player;
	protected AudioStreamPlayer musicPlayer;
	protected Sprite2D backgroundSprite;
	protected Timer stageTimer;

	public override void _Ready()
	{
		InitializeStage();
	}

	/// <summary>
	/// Inicialização padronizada de todas as stages.
	/// Ordem: validação → setup → spawn → lógica específica → feedback
	/// </summary>
	private void InitializeStage()
	{
		ValidateStageData();
		SetupBackground();
		PlayMusic();
		SetupTimer();
		SpawnPlayer();
		OnStageReady();
		ShowStageName();

		GD.Print($"Stage '{Data?.StageName ?? "Unknown"}' inicializada");
	}

	/// <summary>
	/// Hook para lógica específica de cada stage.
	/// Chamado após toda a inicialização padrão.
	/// </summary>
	protected virtual void OnStageReady() { }
	
	private void SpawnPlayer()
	{
		var spawnPoint = GetNode<Marker2D>("SpawnPoint");

		if(spawnPoint == null)
		{
			GD.PrintErr("SpawnPoint não encontrado na stage!");
			return;
		}

		GD.Print($"SpawnPoint encontrado na posição: {spawnPoint.GlobalPosition}");

		// Carrega prefab do player
		var playerScene = GD.Load<PackedScene>("res://entities/player/PlayerMain.tscn");
		player = playerScene.Instantiate<PlayerMain>();
		player.GlobalPosition = spawnPoint.GlobalPosition;

		AddChild(player);

		GD.Print($"Player spawnado na posição: {player.GlobalPosition}");

		// Conecta morte do player
		player.GetNode<CombatController>("CombatController").Died += OnPlayerDied;
	}
	
	private void PlayMusic()
	{
		if(Data == null || Data.Music == null) return;
		
		musicPlayer = new AudioStreamPlayer
		{
			Stream = Data.Music,
			Autoplay = true,
			Bus = "Music"
		};
		
		AddChild(musicPlayer);
	}
	
	protected void CompleteStage()
	{
		EmitSignal(SignalName.StageCompleted);
	}

	protected void DefeatBoss()
	{
		EmitSignal(SignalName.BossDefeated);
	}

	/// <summary>
	/// Obtém um nó obrigatório, com validação e erro se não encontrado.
	/// </summary>
	protected T GetRequiredNode<T>(string path) where T : Node
	{
		var node = GetNode<T>(path);
		if (node == null)
		{
			GD.PrintErr($"Required node not found: {path} in stage {Data?.StageName ?? "Unknown"}");
		}
		return node;
	}

	/// <summary>
	/// Mostra o nome da stage no centro da tela (estilo NES)
	/// </summary>
	protected virtual void ShowStageName()
	{
		if(Data?.StageName == null) return;

		// Cria container para o nome da stage
		var canvasLayer = new CanvasLayer { Layer = 10 };
		var control = new Control();
		control.SetAnchorsPreset(Control.LayoutPreset.FullRect);

		// Background semi-transparente (estilo NES)
		var background = new ColorRect
		{
			Color = new Color(0, 0, 0, 0.7f),
			Size = new Vector2(300, 80)
		};
		background.Position = new Vector2(
			(GetViewport().GetVisibleRect().Size.X - background.Size.X) / 2,
			(GetViewport().GetVisibleRect().Size.Y - background.Size.Y) / 2
		);
		control.AddChild(background);

		// Label do nome da stage
		var stageNameLabel = new Label
		{
			Text = Data.StageName,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		};

		// Configurar aparencia usando overrides de tema
		stageNameLabel.AddThemeFontSizeOverride("font_size", 24);
		stageNameLabel.AddThemeColorOverride("font_color", new Color(1, 1, 0.8f)); // Branco amarelado
		stageNameLabel.Position = background.Position + new Vector2(0, 25);
		stageNameLabel.Size = background.Size;
		control.AddChild(stageNameLabel);

		canvasLayer.AddChild(control);
		AddChild(canvasLayer);

		// Animação de entrada dramática (estilo NES)
		var tween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);

		// Começa invisível e pequeno
		var startScale = new Vector2(0.5f, 0.5f);
		var endScale = new Vector2(1.2f, 1.2f);
		background.Scale = startScale;
		stageNameLabel.Scale = startScale;
		background.Modulate = new Color(1, 1, 1, 0);
		stageNameLabel.Modulate = new Color(1, 1, 1, 0);

		// Anima entrada
		tween.TweenProperty(background, "scale", endScale, 0.3f);
		tween.Parallel().TweenProperty(stageNameLabel, "scale", endScale, 0.3f);
		tween.Parallel().TweenProperty(background, "modulate:a", 1.0f, 0.3f);
		tween.Parallel().TweenProperty(stageNameLabel, "modulate:a", 1.0f, 0.3f);

		// Volta ao tamanho normal
		tween.TweenProperty(background, "scale", Vector2.One, 0.2f);
		tween.Parallel().TweenProperty(stageNameLabel, "scale", Vector2.One, 0.2f);

		// Mantém visível por 2.5 segundos
		tween.TweenInterval(2.5f);

		// Anima saída
		tween.TweenProperty(background, "modulate:a", 0.0f, 0.5f);
		tween.Parallel().TweenProperty(stageNameLabel, "modulate:a", 0.0f, 0.5f);
		tween.TweenCallback(Callable.From(() => canvasLayer.QueueFree()));
	}

	private void OnPlayerDied()
	{
		GameManager.Instance.OnPlayerDied();
	}

	/// <summary>
	/// Valida se os dados da stage estão corretos
	/// </summary>
	private void ValidateStageData()
	{
		if(Data == null)
		{
			GD.PrintErr("StageData não definido! Usando valores padrão.");
			Data = new StageData();
			return;
		}

		// Validações básicas
		if(string.IsNullOrEmpty(Data.StageName))
		{
			GD.PushWarning("StageName não definido, usando nome padrão.");
			Data.StageName = "Unnamed Stage";
		}

		if(Data.TimeLimit <= 0)
		{
			GD.PushWarning("TimeLimit inválido, definindo como 300 segundos.");
			Data.TimeLimit = 300;
		}

		if(Data.CompletionBonus < 0)
		{
			GD.PushWarning("CompletionBonus negativo, definindo como 0.");
			Data.CompletionBonus = 0;
		}
	}

	/// <summary>
	/// Configura o background da stage se definido
	/// </summary>
	private void SetupBackground()
	{
		if(Data?.Background == null) return;

		backgroundSprite = new Sprite2D
		{
			Texture = Data.Background,
			Centered = false,
			ZIndex = -10 // Atrás de tudo
		};

		// Configura para cobrir a tela (ajuste conforme necessário)
		backgroundSprite.Scale = new Vector2(4, 4); // Ajuste de escala NES
		AddChild(backgroundSprite);
	}

	/// <summary>
	/// Configura timer da stage se houver limite de tempo
	/// </summary>
	private void SetupTimer()
	{
		if(Data?.TimeLimit <= 0) return;

		stageTimer = new Timer
		{
			WaitTime = Data.TimeLimit,
			OneShot = true,
			Autostart = true
		};

		stageTimer.Timeout += OnStageTimeUp;
		AddChild(stageTimer);
	}

	/// <summary>
	/// Chamado quando o tempo da stage acaba
	/// </summary>
	protected virtual void OnStageTimeUp()
	{
		GD.Print($"Tempo esgotado na stage '{Data?.StageName}'!");
		// Implementação padrão: mata o player ou força restart
		if(player != null)
		{
			player.GetNode<CombatController>("CombatController").TakeDamage(
				CombatController.DamageInfo.Create(
					999, // Dano fatal
					player.GlobalPosition,
					200f // Knockback forte para morte
				)
			);
		}
	}

	/// <summary>
	/// Retorna o tempo restante da stage (em segundos)
	/// </summary>
	public float GetRemainingTime()
	{
		if(stageTimer == null) return -1;
		return (float)stageTimer.TimeLeft;
	}

	/// <summary>
	/// Retorna se a stage tem boss
	/// </summary>
	public bool HasBoss()
	{
		return Data?.HasBoss ?? false;
	}

	/// <summary>
	/// Retorna se a stage tem checkpoint
	/// </summary>
	public bool HasCheckpoint()
	{
		return Data?.HasCheckpoint ?? false;
	}

	/// <summary>
	/// Retorna o bônus de completion da stage
	/// </summary>
	public int GetCompletionBonus()
	{
		return Data?.CompletionBonus ?? 0;
	}
}
