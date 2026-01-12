using Godot;
using System;

/// <summary>
/// Gerencia transições estilo NES (fade to black).
/// </summary>
public partial class TransitionManager : CanvasLayer
{
	private ColorRect fadeRect;
	private Tween currentTween;
	
	public override void _Ready()
	{
		Layer = 100; // Sempre por cima de tudo
		
		// Retângulo preto que cobre a tela
		fadeRect = new ColorRect
		{
			Color = Colors.Black,
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		
		// Preenche tela inteira
		fadeRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		AddChild(fadeRect);
		
		// Começa invisível
		fadeRect.Modulate = new Color(1, 1, 1, 0);
	}
	
	/// <summary>
	/// Fade para preto (NES style: instantâneo ou 2-3 frames)
	/// </summary>
	public void FadeOut(Action onComplete = null)
	{
		currentTween?.Kill();
		
		currentTween = CreateTween();
		currentTween.TweenProperty(fadeRect, "modulate:a", 1.0f, 0.15f); // ~9 frames a 60fps
		
		if(onComplete != null)
		{
			currentTween.TweenCallback(Callable.From(onComplete));
		}
	}
	
	/// <summary>
	/// Fade de preto para transparente
	/// </summary>
	public void FadeIn(Action onComplete = null)
	{
		currentTween?.Kill();
		
		currentTween = CreateTween();
		currentTween.TweenProperty(fadeRect, "modulate:a", 0.0f, 0.15f);
		
		if(onComplete != null)
		{
			currentTween.TweenCallback(Callable.From(onComplete));
		}
	}
	
	/// <summary>
	/// Fade rápido (morte/respawn)
	/// </summary>
	public void QuickFade(Action onMidpoint = null)
	{
		currentTween?.Kill();
		
		currentTween = CreateTween();
		currentTween.TweenProperty(fadeRect, "modulate:a", 1.0f, 0.08f);
		
		if(onMidpoint != null)
		{
			currentTween.TweenCallback(Callable.From(onMidpoint));
		}
		
		currentTween.TweenProperty(fadeRect, "modulate:a", 0.0f, 0.08f);
	}
	
	/// <summary>
	/// Tela preta instantânea (Game Over, Boss Intro)
	/// </summary>
	public void InstantBlack()
	{
		currentTween?.Kill();
		fadeRect.Modulate = new Color(1, 1, 1, 1);
	}
	
	public void InstantClear()
	{
		currentTween?.Kill();
		fadeRect.Modulate = new Color(1, 1, 1, 0);
	}
}
