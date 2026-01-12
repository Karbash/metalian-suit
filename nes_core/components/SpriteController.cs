using Godot;

public partial class SpriteController : Node2D
{
	[Signal] public delegate void AnimationCompleteEventHandler(string animationName);
	[Signal] public delegate void FrameChangedEventHandler(int frame);
	
	private AnimatedSprite2D sprite;
	private Timer hurtFlashTimer;
	private bool isFlashing;
	private string currentAnimation = "";
	
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		
		hurtFlashTimer = new Timer();
		AddChild(hurtFlashTimer);
		hurtFlashTimer.OneShot = true;
		hurtFlashTimer.Timeout += () => 
		{
			isFlashing = false;
			sprite.Modulate = Colors.White;
		};
		
		sprite.AnimationFinished += () => EmitSignal(SignalName.AnimationComplete, currentAnimation);
		sprite.FrameChanged += () => EmitSignal(SignalName.FrameChanged, sprite.Frame);
	}
	
	public void Play(string animation, bool facingRight, bool forceRestart = false)
	{
		sprite.FlipH = !facingRight;
		
		if(currentAnimation != animation || forceRestart)
		{
			sprite.Play(animation);
			currentAnimation = animation;
		}
	}
	
	public void StartHurtFlash(float duration)
	{
		isFlashing = true;
		hurtFlashTimer.Start(duration);
	}
	
	public int GetCurrentFrame() => sprite.Frame;

	public string GetCurrentAnimation() => currentAnimation;
	
	public override void _Process(double delta)
	{
		if(isFlashing)
		{
			sprite.Modulate = (Engine.GetPhysicsFrames() % 2 == 0) 
				? Colors.White 
				: new Color(1, 0.3f, 0.3f);
		}
	}
}
