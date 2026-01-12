using System.Collections.Generic;

public class StateMachine
{
	private State currentState;
	private Dictionary<string, State> states = new();
	
	public void AddState(string name, State state)
	{
		states[name] = state;
	}
	
	public void ChangeState(string name)
	{
		if(!states.ContainsKey(name))
		{
			Godot.GD.PrintErr($"State '{name}' não existe!");
			return;
		}

		// Salva estado anterior se for um enemy
		if(currentState?.Entity is Enemy enemy)
		{
			foreach(var kvp in states)
			{
				if(kvp.Value == currentState)
				{
					enemy.SetPreviousState(kvp.Key);
					break;
				}
			}
		}

		currentState?.Exit();
		currentState = states[name];
		currentState.Enter();
	}
	
	public void Update(double delta)
	{
		currentState?.Update(delta);
	}
	
	public string GetCurrentStateName()
	{
		foreach(var kvp in states)
		{
			if(kvp.Value == currentState)
				return kvp.Key;
		}
		return "";
	}
}
