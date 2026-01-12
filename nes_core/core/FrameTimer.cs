/// <summary>
/// Timer baseado em frames, não em segundos.
/// NES real funcionava assim.
/// </summary>

public class FrameTimer
{
	private int frames;
	
	public bool Done => frames <= 0;
	public int Remaining => frames;
	
	public FrameTimer(int frames)
	{
		this.frames = frames;
	}
	
	public void Tick()
	{
		if(frames > 0)
			frames--;
	}
	
	public void Reset(int value)
	{
		frames = value;
	}
	
	public void Add(int value)
	{
		frames += value;
	}
}
