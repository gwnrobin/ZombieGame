using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    ZombieAI context;

    public PlayerStateFactory(ZombieAI currentContext)
    {
        this.context = currentContext;
    }

    public PlayerBaseState Wander() 
    {
        return new WanderState(context, this);
    }
    public PlayerBaseState Attack()
    {
        return new AttackState(context, this);
    }
    public PlayerBaseState Detect()
    {
        return new DetectedState(context, this);
    }
}
