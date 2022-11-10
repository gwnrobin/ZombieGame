using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : PlayerBaseState
{
    private float wanderTimer = 0;

    public WanderState(ZombieAI currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        isRootState = true;
    }

    public override void EnterState()
    {
        ctx.animator.SetBool("run", false);

        ctx.navMeshAgent.speed = ctx.wanderSpeed;
        ctx.view.radius = ctx.normalDetectRange;
        ctx.view.angle = ctx.viewAngle;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (ctx.navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= 10 + Random.Range(0, 30))
            {
                ctx.SetPath();
                wanderTimer = 0;
            }
        }

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
        if (ctx.seePlayer)
        {
            SwitchStates(factory.Detect());
            ctx.Scream.Send();
        }
    }
}
