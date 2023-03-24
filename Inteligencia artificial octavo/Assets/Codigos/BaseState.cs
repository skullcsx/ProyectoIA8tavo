using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    public string name;
    public StateMachine stateMachine;

    public BaseState(string name, StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }

    public void PrintName()
    {
        Debug.Log("state name is " + name);
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void UpdateLogic()
    {

    }

    public virtual void UpdatePhysics()
    {

    }


}
