using HQFPSTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerBaseState
{
    public AttackState(ZombieAI currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        isRootState = true;
    }

    public override void EnterState()
    {
        ctx.animator.SetBool("attack", true);
    }

    private void Attack()
    {
        if (Physics.SphereCast(ctx.attackOrigin.position, .2f, ctx.attackOrigin.forward, out RaycastHit hitInfo, 1.5f, ctx.HitMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log(hitInfo.collider.gameObject);
            DamageInfo damageInfo = new DamageInfo(-10, DamageType.Stab, ctx, hitInfo.transform);

            // Try to damage the Hit object
            ctx.DealDamage.Try(damageInfo, null);
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (ctx.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % .5f > 0.48)
        {
            Attack();
        }
    }

    public override void ExitState()
    {
        ctx.animator.SetBool("attack", false);
    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchStates()
    {
        if(ctx.navMeshAgent.remainingDistance > ctx.navMeshAgent.stoppingDistance)
        {
            SwitchStates(factory.Detect());
        }
        if (ctx.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.98)
        {
            SwitchStates(factory.Attack());
        }
    }
}
