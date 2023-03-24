using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    BaseState currentState;
    // Start is called before the first frame update
    public void Start()
    {
        currentState = GetInitialState();
        if(currentState != null)
        {
            currentState.Enter();
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if(currentState != null)
        {
            currentState.UpdateLogic();
        }
    }

    public void FixedUpdate()
    {
        if(currentState != null)
        {
            currentState.UpdatePhysics();
        }
    }

    public void ChangeState(BaseState newState)
    {
        //
        currentState.Exit();

        currentState = newState;

        currentState.Enter();
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    private void OnGUI()
    {
        //if de una sola linea
        string text = currentState != null ? currentState.name : "No Current State asigned";
        GUILayout.Label($"<size=40>{text}</size>");
    }
}
