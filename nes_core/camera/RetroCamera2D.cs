using Godot;

public partial class RetroCamera2D : Node2D
{
	[Export] public Vector2I ScreenSize = new Vector2I(256, 240); // Tamanho da "tela" NES
	[Export] public bool VerticalScroll = true; // Se permite scroll vertical (habilitado por padrão para jogos de plataforma)
	[Export] public float DeadZone = 0.3f; // Zona morta antes de mover câmera (30% da tela)

	private PlayerMain player;
	private Camera2D camera;
	private Vector2 currentScreen; // Tela atual em coordenadas de grid

	public override void _Ready()
	{
		// Cria e configura a câmera estilo NES (sem smooth, grid-based)
		camera = new Camera2D
		{
			PositionSmoothingEnabled = false, // Estilo NES: sem smooth
			PositionSmoothingSpeed = 0,       // Garante sem smooth
			Enabled = true                    // Ativa a câmera (Godot 4)
		};
		AddChild(camera);

		// Zoom padrão para manter a escala NES
		// ScreenSize define o "grid" de movimento, não o zoom da câmera
		camera.Zoom = new Vector2(1, 1); // Zoom 1:1 por padrão

		// Torna esta câmera a atual (estilo Castlevania)
		var viewport = GetViewport();
		if(viewport != null)
		{
			viewport.GetCamera2D()?.MakeCurrent(); // Desativa câmera atual se existir
		}

		camera.MakeCurrent();

		// Inicializa na tela (0,0) - será atualizado quando o player for encontrado
		currentScreen = Vector2.Zero;

		// Verifica se já existe player no grupo
		var existingPlayer = GetTree().GetFirstNodeInGroup("player") as PlayerMain;
		if (existingPlayer != null)
		{
			player = existingPlayer;
			InitializeCameraToPlayer();
		}
	}
	
	public override void _Process(double delta)
	{
		if(camera == null)
		{
			return;
		}

		if(player == null)
		{
			player = GetTree().GetFirstNodeInGroup("player") as PlayerMain;
			if(player == null)
			{
				return;
			}

			// Quando encontra o player pela primeira vez, posiciona a câmera nele
			InitializeCameraToPlayer();
		}

		// Sistema de câmera estilo Castlevania/NES com dead zone
		var playerPos = player.GlobalPosition;
		var cameraPos = camera.Position;


		// Calcula posição relativa do jogador na tela atual da câmera
		var relativeX = playerPos.X - cameraPos.X;
		var relativeY = VerticalScroll ? (playerPos.Y - cameraPos.Y) : 0;

		// Dead zone: zona onde o jogador pode se mover sem mover a câmera
		var deadZoneWidth = ScreenSize.X * DeadZone;
		var deadZoneHeight = VerticalScroll ? (ScreenSize.Y * DeadZone) : 0;

		var shouldMoveX = Mathf.Abs(relativeX) > (ScreenSize.X / 2 - deadZoneWidth / 2);
		var shouldMoveY = VerticalScroll && Mathf.Abs(relativeY) > (ScreenSize.Y / 2 - deadZoneHeight / 2);

		if(shouldMoveX || shouldMoveY)
		{
			// Calcula nova tela baseada na posição do jogador
			var playerScreenX = Mathf.Floor(playerPos.X / ScreenSize.X);
			var playerScreenY = VerticalScroll ?
				Mathf.Floor(playerPos.Y / ScreenSize.Y) : currentScreen.Y;

			var targetScreen = new Vector2(playerScreenX, playerScreenY);

			// Só move se realmente mudou de tela
			if(currentScreen != targetScreen)
			{
				// Nova posição da câmera (centro da nova tela)
				var targetX = targetScreen.X * ScreenSize.X + ScreenSize.X / 2;
				var targetY = VerticalScroll ?
					targetScreen.Y * ScreenSize.Y + ScreenSize.Y / 2 : cameraPos.Y;

				var newPos = new Vector2(targetX, targetY);

				camera.Position = newPos;
				currentScreen = targetScreen;

		// Verifica se a câmera ainda é a atual através do viewport
		var viewport = GetViewport();
		if(viewport != null && viewport.GetCamera2D() != camera)
		{
			camera.MakeCurrent();
		}
			}
		}
	}

	/// <summary>
	/// Inicializa a câmera na posição do jogador quando ele é encontrado
	/// Estratégia híbrida: começa centrada no jogador, depois usa grid NES
	/// </summary>
	private void InitializeCameraToPlayer()
	{
		if(player == null || camera == null) return;

		var playerPos = player.GlobalPosition;

		// Estratégia: começa centrada no jogador para garantir visibilidade
		// Depois o sistema de grid NES toma conta
		var cameraX = playerPos.X;
		var cameraY = VerticalScroll ? playerPos.Y : camera.Position.Y;

		// Calcula qual tela isso representa (para consistência futura)
		var initialScreenX = Mathf.Floor(cameraX / ScreenSize.X);
		var initialScreenY = VerticalScroll ? Mathf.Floor(cameraY / ScreenSize.Y) : currentScreen.Y;

		currentScreen = new Vector2(initialScreenX, initialScreenY);
		camera.Position = new Vector2(cameraX, cameraY);
	}
}
