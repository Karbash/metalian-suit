using Godot;

public partial class GameHUD : CanvasLayer
{
	private Label livesLabel;
	private Label scoreLabel;
	private ProgressBar bossHealthBar;
	
	public override void _Ready()
	{
		livesLabel = GetNode<Label>("LivesLabel");
		scoreLabel = GetNode<Label>("ScoreLabel");
		bossHealthBar = GetNode<ProgressBar>("BossHealthBar");
		
		bossHealthBar.Visible = false;
		
		// Conecta aos signals do GameManager
		GameManager.Instance.LivesChanged += UpdateLives;
		GameManager.Instance.ScoreChanged += UpdateScore;
		
		// Inicializa
		UpdateLives(GameManager.Instance.Lives);
		UpdateScore(GameManager.Instance.Score);
	}
	
	private void UpdateLives(int lives)
	{
		livesLabel.Text = $"LIVES: {lives}";
	}
	
	private void UpdateScore(int score)
	{
		// NES: score sempre com zeros à esquerda
		scoreLabel.Text = score.ToString("D8"); // 00012340
	}
	
	public void ShowBossHealth(int maxHealth)
	{
		bossHealthBar.Visible = true;
		bossHealthBar.MaxValue = maxHealth;
		bossHealthBar.Value = maxHealth;
	}
	
	public void UpdateBossHealth(int health)
	{
		bossHealthBar.Value = health;
	}
	
	public void HideBossHealth()
	{
		bossHealthBar.Visible = false;
	}
}
