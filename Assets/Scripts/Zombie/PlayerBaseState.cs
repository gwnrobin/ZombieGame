using UnityEngine;

public abstract class PlayerBaseState
{
    protected bool isRootState = false;
    protected ZombieAI ctx;
    protected PlayerStateFactory factory;
    protected PlayerBaseState currentSubState;
    protected PlayerBaseState currentSuperState;

    public string animationName;

    public PlayerBaseState(ZombieAI currentContext, PlayerStateFactory playerStateFactory)
    {
        ctx = currentContext;
        factory = playerStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates() 
    {
        UpdateState();
        if(currentSubState != null)
        {
            currentSubState.UpdateStates();
        }
    }

    protected void SwitchStates(PlayerBaseState newState) 
    {
        ExitState();

        newState.EnterState();

        if(isRootState)
        {
            ctx.CurrentState = newState;
        }
        else if (currentSuperState != null)
        {
            currentSuperState.SetSubStates(newState);
        }
    }
    protected void SetSuperState(PlayerBaseState newSuperState) 
    {
        currentSuperState = newSuperState;
    }
    protected void SetSubStates(PlayerBaseState newSubState) 
    {
        currentSubState = newSubState;
        currentSubState.SetSuperState(this);
        currentSubState.EnterState();
    }
}
