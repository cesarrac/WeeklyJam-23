using UnityEngine;
using System.Collections;

public enum StateType
{
    Idle, Jump, Transit, StationArrival, StationInside, StationExit, MissionComplete
}

public abstract class State
{
    public StateType stateType { get; protected set; }

    public State (StateType sType)
    {
        stateType = sType;
    }

    public abstract void Update(float deltaTime);

    public virtual void Enter()
    {
        Debug.Log("Enter " + stateType);
    }

    public virtual void Exit()
    {
        Debug.Log("Exit " + stateType);
    }

    public virtual void Interrupt()
    {
        Debug.Log("Interrupt " + stateType);
    }

    public virtual void Finished()
    {
        Debug.Log("Finished " + stateType);
    }
}
