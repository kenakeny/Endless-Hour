// ============ CorenStateMachine.cs ============
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorenStateMachine : MonoBehaviour
{
    public CorenState activeState;
    public CorenPhase1 phase1State;
    public CorenPhase2 phase2State;

    void Start()
    {
        CorenBoss coren = GetComponent<CorenBoss>();
        phase1State = GetComponent<CorenPhase1>();
        phase2State = GetComponent<CorenPhase2>();
        
        phase1State.Initialize(coren, this);
        phase2State.Initialize(coren, this);

        ChangeState(phase1State);
    }

    public void ChangeState(CorenState newState)
    {
        if (activeState != null)
            activeState.Exit();

        activeState = newState;

        if (activeState != null)
            activeState.EnterState();
    }

    // Renamed from Update() so Unity does NOT auto-call it as a lifecycle message.
    // It is driven manually from CorenBoss.Update() — otherwise the state would
    // tick twice per frame (attacks ran at 2x speed).
    public void Tick()
    {
        if (activeState != null)
            activeState.UpdateState();
    }
}
