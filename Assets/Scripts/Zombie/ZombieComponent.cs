using HQFPSTemplate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieComponent : EntityComponent
{
	public ZombieAI zombie
	{
		get
		{
			if (!m_Zombie)
				m_Zombie = GetComponent<ZombieAI>();
			if (!m_Zombie)
				m_Zombie = GetComponentInParent<ZombieAI>();

			return m_Zombie;
		}
	}

	private ZombieAI m_Zombie;
}

