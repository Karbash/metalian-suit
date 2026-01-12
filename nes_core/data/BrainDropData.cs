using Godot;

/// <summary>
/// Configuração de drop de Alien Brain ao matar inimigo.
/// Estilo Metroid - inimigos dropam power-ups.
/// </summary>
[GlobalClass]
public partial class BrainDropData : Resource
{
	[Export] public PackedScene BrainScene;
	[Export(PropertyHint.Range, "0,1,0.01")]
	public float DropChance = 0.75f;
	
	public void Spawn(Vector2 position)
	{
		if(BrainScene == null) return;
		
		var brain = BrainScene.Instantiate<Node2D>();
		brain.GlobalPosition = position;
		
		// Adiciona à cena raiz
		var root = Engine.GetMainLoop() as SceneTree;
		root?.Root.AddChild(brain);
	}
}
