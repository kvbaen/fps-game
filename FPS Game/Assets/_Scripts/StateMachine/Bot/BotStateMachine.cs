using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotStateMachine : StateManager<BotStateMachine.BotState>
{
    public enum BotState
    {
        Idle,
        Patrolling,
        ChasingPlayer,
        Attacking
    }

    private void Awake()
    {
        CurrentState = States[BotState.Idle];
    }
}
