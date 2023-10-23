using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotIdleState : BaseState<BotStateMachine.BotState>
{
    public new BotStateMachine.BotState StateKey;

    public BotIdleState(BotStateMachine.BotState key) : base(key)
    {
        StateKey = key;
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override BotStateMachine.BotState GetNextState()
    {
        throw new System.NotImplementedException();
    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }

    public override void OnTriggerStay(Collider other)
    {

    }

    public override void UpdateState()
    {

    }
}
