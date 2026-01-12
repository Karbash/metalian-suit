using Godot;

public partial class Checkpoint : Area2D
{
	[Signal] public delegate void ActivatedEventHandler(Vector2 position);
	
	private bool activated = false;
	private AnimatedSprite2D sprite;
	
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		BodyEntered += OnBodyEntered;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if(activated) return;
		
		if(body is PlayerMain)
		{
			activated = true;
			sprite.Play("activated");
			
			EmitSignal(SignalName.Activated, GlobalPosition);
			
			// TODO: Som de checkpoint
		}
	}
}
