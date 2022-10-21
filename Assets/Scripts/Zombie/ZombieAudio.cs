using HQFPSTemplate;
using HQFPSTemplate.Surfaces;
using System;
using UnityEngine;

public class ZombieAudio : ZombieComponent
{
	[Serializable]
	private struct PlayerVitalsAudio
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

		[BHeader("Stamina", true)]

		[Group]
		public SoundPlayer BreathingHeavyAudio;

		public float BreathingHeavyDuration;
	}

	[Serializable]
	private struct PlayerFootstepsAudio
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

	[SerializeField]
	private AudioSource m_AudioSource = null;

	[SerializeField, Group]
	private PlayerVitalsAudio m_PlayerVitalsAudio = new PlayerVitalsAudio();

	[SerializeField, Group]
	private PlayerFootstepsAudio m_PlayerFootsteps = new PlayerFootstepsAudio();

	private float m_NextTimeCanScream;

	private void Start()
	{
		//zombie.MoveCycleEnded.AddListener(PlayFootstep);

		zombie.Death.AddListener(() => { m_PlayerVitalsAudio.DeathAudio.Play(m_AudioSource); });

		zombie.Health.AddChangeListener(OnChanged_Health);

	}

	private void Update()
	{
		AudioListener.volume = Mathf.MoveTowards(AudioListener.volume, 1f, m_PlayerVitalsAudio.EarRingVolumeGainSpeed * Time.deltaTime);
	}

	private void OnChanged_Health(float health)
	{
		float delta = health - Entity.Health.GetPreviousValue();

		if (delta < 0f)
		{
			if (Time.time > m_NextTimeCanScream)
			{
				m_PlayerVitalsAudio.HurtAudio.Play(ItemSelection.Method.RandomExcludeLast, m_AudioSource);
				m_NextTimeCanScream = Time.time + m_PlayerVitalsAudio.TimeBetweenScreams;
			}
		}
	}

	private void PlayFootstep()
	{
		if (zombie.Velocity.Val.sqrMagnitude > 0.1f)
		{
			SurfaceEffects footstepEffect = SurfaceEffects.SoftFootstep;

			float volumeFactor = m_PlayerFootsteps.WalkVolume;

			RaycastHit hitInfo;

			if (CheckGround(out hitInfo))
				SurfaceManager.SpawnEffect(hitInfo, footstepEffect, volumeFactor);
		}
	}

	private bool CheckGround(out RaycastHit hitInfo)
	{
		Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

		return Physics.Raycast(ray, out hitInfo, m_PlayerFootsteps.RaycastDistance, m_PlayerFootsteps.GroundMask, QueryTriggerInteraction.Ignore);
	}
}
