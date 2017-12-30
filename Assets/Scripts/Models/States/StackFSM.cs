using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackFSM {

    Stack<State> StateStack;

    public StackFSM()
    {
        StateStack = new Stack<State>();
    }

    public void Push(State stateToPush, bool entersState = true)
    {
        //Debug.Log("Push");
        if (stateToPush == null)
            return;

        if (GetCurrentState() != stateToPush)
        {
            StateStack.Push(stateToPush);

            // Enter the State
            if (GetCurrentState() != null && entersState == true)
                GetCurrentState().Enter();

        }
    }
    public void Replace(State stateToPush){
        GetCurrentState().Exit();
        StateStack.Pop();
        Push(stateToPush);
    }
    public void Pop()
    {
        GetCurrentState().Exit();
        StateStack.Pop();

        // Enter the State
        if (GetCurrentState() != null)
            GetCurrentState().Enter();
    }

    public State GetCurrentState()
    {
        if (StateStack == null)
            return null;

        return StateStack.Count > 0 ? StateStack.Peek() : null;
    }

    public void Reset()
    {
        if (StateStack == null)
            return;

        StateStack.Clear();
    }
}
