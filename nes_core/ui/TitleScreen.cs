using Godot;

public partial class TitleScreen : Control
{
	private InputController input;

	public override void _Ready()
	{
		// Tenta acessar InputController como autoload
		input = GetNodeOrNull<InputController>("/root/InputController");

		if(input == null)
		{
			GD.Print("InputController não encontrado, tentando novamente em _Process");
		}
	}

	public override void _Process(double delta)
	{
		// Tenta novamente se não conseguiu no _Ready
		if(input == null)
		{
			input = GetNodeOrNull<InputController>("/root/InputController");
			if(input == null) return;
		}

		if(input.IsJustPressed(InputController.NESButton.Start))
		{
			StartGame();
		}
	}
	
	private void StartGame()
	{
		// Usa SceneTree direto para evitar null após esta cena ser liberada
		var tree = Engine.GetMainLoop() as SceneTree;
		if(tree == null) return;

		tree.CreateTimer(0).Timeout += () =>
		{
			tree.ChangeSceneToFile("res://Game.tscn");
			tree.CreateTimer(0).Timeout += () => GameManager.Instance.StartNewGame();
		};
	}
}
//
//
//1. TitleScreen.tscn
   //↓ (Press Start)
   //
//2. Game.tscn (carrega)
   //├── GameHUD (visível)
   //├── StageContainer (vazio)
   //└── MusicPlayer
   //
//3. GameManager.StartNewGame()
   //↓
   //
//4. StageManager.LoadStage(0)
   //↓ (Fade Out)
   //
//5. Stage1.tscn instanciada em StageContainer
   //├── Player spawna
   //├── Música toca
   //└── HUD atualiza
   //↓ (Fade In)
   //
//6. Gameplay...
   //├── Player morre → GameManager.OnPlayerDied()
   //│   ├── Lives-- 
   //│   └── Se Lives > 0: Respawn/Reload
   //│   └── Se Lives = 0: Game Over
   //│
   //├── Checkpoint → GameManager.SetCheckpoint()
   //│
   //└── Goal → Stage.CompleteStage()
	   //↓
	   //
//7. GameManager.LoadNextStage()
   //↓ (Fade Out)
   //
//8. Stage1 destruída
   //↓
   //
//9. Stage2 instanciada
   //(HUD/Music/GameManager persistem)
