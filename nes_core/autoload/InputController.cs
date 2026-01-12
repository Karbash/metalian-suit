using Godot;
using System.Collections.Generic;

public partial class InputController : Node
{
	public enum NESButton
	{
		A, B, Select, Start, Up, Down, Left, Right
	}
	
	private static readonly Dictionary<NESButton, string> InputMap = new()
	{
		{ NESButton.A, "nes_a" },
		{ NESButton.B, "nes_b" },
		{ NESButton.Select, "nes_select" },
		{ NESButton.Start, "nes_start" },
		{ NESButton.Up, "nes_up" },
		{ NESButton.Down, "nes_down" },
		{ NESButton.Left, "nes_left" },
		{ NESButton.Right, "nes_right" }
	};
	
	public bool IsPressed(NESButton button) 
		=> Input.IsActionPressed(InputMap[button]);
	
	public bool IsJustPressed(NESButton button) 
		=> Input.IsActionJustPressed(InputMap[button]);
	
	public bool IsJustReleased(NESButton button) 
		=> Input.IsActionJustReleased(InputMap[button]);
	
	public int GetHorizontalAxis()
	{
		bool left = IsPressed(NESButton.Left);
		bool right = IsPressed(NESButton.Right);
		
		if(left && right)
			return IsJustPressed(NESButton.Right) ? 1 : 
				   IsJustPressed(NESButton.Left) ? -1 : 0;
		
		if(right) return 1;
		if(left) return -1;
		return 0;
	}
	
	public int GetVerticalAxis()
	{
		bool up = IsPressed(NESButton.Up);
		bool down = IsPressed(NESButton.Down);
		
		if(up && down)
			return IsJustPressed(NESButton.Down) ? 1 : 
				   IsJustPressed(NESButton.Up) ? -1 : 0;
		
		if(down) return 1;
		if(up) return -1;
		return 0;
	}
}
