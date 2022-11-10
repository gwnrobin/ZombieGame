using HQFPSTemplate;
using HQFPSTemplate.Surfaces;
using System;
using UnityEngine;

public class ZombieAudio : ZombieComponent
{
	[Serializable]
	private struct ZombieVitalsAudio
	{
		[BHeader("Health", true)]

		[Group]
		[Tooltip("The sounds that will be played when this entity receives damage.")]
		public SoundPlayer HurtAudio;

		[SerializeField]
		public float TimeBetweenScreams;

		[Space]

		[Group]
		public SoundPlayer FallDamageAudio;

		[Space]

		[Group]
		public SoundPlayer EarRingingAudio;

		[Range(0f, 1f)]
		public float EarRingVolumeDecrease;
		public float EarRingVolumeGainSpeed;

		[Space]

		[Group]
		public SoundPlayer DeathAudio;
	}

	[Serializable]
	private struct ZombieFootstepsAudio
	{
		public LayerMask GroundMask;

		[Range(0f, 1f)]
		public float RaycastDistance;

		[Range(0f, 10f)]
		[Tooltip("If the impact speed is higher than this threeshold, an effect will be played.")]
		public float FallImpactThreeshold;

		[Range(0f, 1f)]
		public float WalkVolume;

		[Range(0f, 1f)]
		public float CrouchVolume;

		[Range(0f, 1f)]
		public float ProneVolume;

		[Range(0f, 1f)]
		public float RunVolume;
	}

	[Serializable]
	private struct ZombieScreamAudio
    {
		[BHeader("Scream", false)]

		[Group]
		[Tooltip("The sounds that will be played when this entity receives damage.")]
		public SoundPlayer ScreamAudio;

		[SerializeField]
		public float TimeBetweenScreams;
	}

	[SerializeField]
	private AudioSource m_AudioSource = null;

	[SerializeField, Group]
	private ZombieVitalsAudio m_ZombieVitalsAudio = new ZombieVitalsAudio();

	[SerializeField, Group]
	private ZombieFootstepsAudio m_ZombieFootsteps = new ZombieFootstepsAudio();

	[SerializeField, Group]
	private ZombieScreamAudio m_ZombieScreams = new ZombieScreamAudio();

	private float m_NextTimeCanScream;
	private float m_NextTimeCanScreamAngry;

	private void Start()
	{
		zombie.MoveCycleEnded.AddListener(PlayFootstep);

		zombie.Death.AddListener(() => { m_ZombieVitalsAudio.DeathAudio.Play(m_AudioSource); });

		zombie.Health.AddChangeListener(OnChanged_Health);

		zombie.Scream.AddListener(PlayScream);
	}

	private void OnChanged_Health(float health)
	{
		float delta = health - Entity.Health.GetPreviousValue();

		if (delta < 0f)
		{
			if (Time.time > m_NextTimeCanScream)
			{
				m_ZombieVitalsAudio.HurtAudio.Play(ItemSelection.Method.RandomExcludeLast, m_AudioSource);
				m_NextTimeCanScream = Time.time + m_ZombieVitalsAudio.TimeBetweenScreams;
			}
		}
	}

	private void PlayFootstep()
	{
		if (zombie.Velocity.Val.sqrMagnitude > 0.1f)
		{
			SurfaceEffects footstepEffect = SurfaceEffects.SoftFootstep;

			float volumeFactor = m_ZombieFootsteps.WalkVolume;

			RaycastHit hitInfo;
			if (CheckGround(out hitInfo))
				SurfaceManager.SpawnEffect(hitInfo, footstepEffect, volumeFactor);
		}
	}

	private void PlayScream()
    {
		if (Time.time > m_NextTimeCanScreamAngry)
		{
			m_ZombieScreams.ScreamAudio.Play(ItemSelection.Method.RandomExcludeLast, m_AudioSource);
			m_NextTimeCanScreamAngry = Time.time + m_ZombieScreams.TimeBetweenScreams;
		}
	}

	private bool CheckGround(out RaycastHit hitInfo)
	{
		Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

		return Physics.Raycast(ray, out hitInfo, m_ZombieFootsteps.RaycastDistance, m_ZombieFootsteps.GroundMask, QueryTriggerInteraction.Ignore);
	}
}
