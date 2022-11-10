using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedState : PlayerBaseState
{
    public DetectedState(ZombieAI currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        isRootState = true;
    }

    public override void EnterState()
    {
        ctx.animator.SetBool("run", true);
        ctx.navMeshAgent.speed = ctx.runSpeed;
        ctx.view.radius *= 2;
        ctx.view.angle = 360;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (ctx.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % .5f > 0.48)
        {
            ctx.MoveCycleEnded.Send();
        }
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchStates()
    {
        if(!ctx.seePlayer)
        {
            SwitchStates(factory.Wander());
        }
        if(ctx.navMeshAgent.remainingDistance <= ctx.navMeshAgent.stoppingDistance)
        {
            SwitchStates(factory.Attack());
        }
    }
}
