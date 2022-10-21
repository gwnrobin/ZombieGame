using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDeath : ZombieComponent
{
    public override void OnEntityStart()
    {
        zombie.Health.AddChangeListener(OnChanged_Health);
    }

    private void OnChanged_Health(float health)
    {
        if (health == 0f)
            StartCoroutine(C_OnDeath());
    }

	private IEnumerator C_OnDeath()
	{
		zombie.Death.Send();

        yield return null;
    }
}
