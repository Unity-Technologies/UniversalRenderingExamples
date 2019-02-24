using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    public class AeroplaneAudio : MonoBehaviour
    {

        [Serializable]
        public class AdvancedSetttings // A class for storing the advanced options.
        {
            public float engineMinDistance = 50f;                   // The min distance of the engine audio source.
            public float engineMaxDistance = 1000f;                 // The max distance of the engine audio source.
            public float engineDopplerLevel = 1f;                   // The doppler level of the engine audio source.
            [Range(0f, 1f)] public float engineMasterVolume = 0.5f; // An overall control of the engine sound volume.
            public float windMinDistance = 10f;                     // The min distance of the wind audio source.
            public float windMaxDistance = 100f;                    // The max distance of the wind audio source.
            public float windDopplerLevel = 1f;                     // The doppler level of the wind audio source.
            [Range(0f, 1f)] public float windMasterVolume = 0.5f;   // An overall control of the wind sound volume.
        }

        [SerializeField] private AudioClip m_EngineSound;                     // Looped engine sound, whose pitch and volume are affected by the plane's throttle setting.
        [SerializeField] private float m_EngineMinThrottlePitch = 0.4f;       // Pitch of the engine sound when at minimum throttle.
        [SerializeField] private float m_EngineMaxThrottlePitch = 2f;         // Pitch of the engine sound when at maximum throttle.
        [SerializeField] private float m_EngineFwdSpeedMultiplier = 0.002f;   // Additional multiplier for an increase in pitch of the engine from the plane's speed.
        [SerializeField] private AudioClip m_WindSound;                       // Looped wind sound, whose pitch and volume are affected by the plane's velocity.
        [SerializeField] private float m_WindBasePitch = 0.2f;                // starting pitch for wind (when plane is at zero speed)
        [SerializeField] private float m_WindSpeedPitchFactor = 0.004f;       // Relative increase in pitch of the wind from the plane's speed.
        [SerializeField] private float m_WindMaxSpeedVolume = 100;            // the speed the aircraft much reach before the wind sound reaches maximum volume.
        [SerializeField] private AdvancedSetttings m_AdvancedSetttings = new AdvancedSetttings();// container to make advanced settings appear as rollout in inspector

        private AudioSource m_EngineSoundSource;  // Reference to the AudioSource for the engine.
        private AudioSource m_WindSoundSource;    // Reference to the AudioSource for the wind.
        private AeroplaneController m_Plane;      // Reference to the aeroplane controller.
        private Rigidbody m_Rigidbody;


        private void Awake()
        {
            // Set up the reference to the aeroplane controller.
            m_Plane = GetComponent<AeroplaneController>();
            m_Rigidbody = GetComponent<Rigidbody>();


            // Add the audiosources and get the references.
            m_EngineSoundSource = gameObject.AddComponent<AudioSource>();
            m_EngineSoundSource.playOnAwake = false;
            m_WindSoundSource = gameObject.AddComponent<AudioSource>();
            m_WindSoundSource.playOnAwake = false;

            // Assign clips to the audiosources.
            m_EngineSoundSource.clip = m_EngineSound;
            m_WindSoundSource.clip = m_WindSound;

            // Set the parameters of the audiosources.
            m_EngineSoundSource.minDistance = m_AdvancedSetttings.engineMinDistance;
            m_EngineSoundSource.maxDistance = m_AdvancedSetttings.engineMaxDistance;
            m_EngineSoundSource.loop = true;
            m_EngineSoundSource.dopplerLevel = m_AdvancedSetttings.engineDopplerLevel;

            m_WindSoundSource.minDistance = m_AdvancedSetttings.windMinDistance;
            m_WindSoundSource.maxDistance = m_AdvancedSetttings.windMaxDistance;
            m_WindSoundSource.loop = true;
            m_WindSoundSource.dopplerLevel = m_AdvancedSetttings.windDopplerLevel;

            // call update here to set the sounds pitch and volumes before they actually play
            Update();

            // Start the sounds playing.
            m_EngineSoundSource.Play();
            m_WindSoundSource.Play();
        }


        private void Update()
        {
            // Find what proportion of the engine's power is being used.
            var enginePowerProportion = Mathf.InverseLerp(0, m_Plane.MaxEnginePower, m_Plane.EnginePower);

            // Set the engine's pitch to be proportional to the engine's current power.
            m_EngineSoundSource.pitch = Mathf.Lerp(m_EngineMinThrottlePitch, m_EngineMaxThrottlePitch, enginePowerProportion);

            // Increase the engine's pitch by an amount proportional to the aeroplane's forward speed.
            // (this makes the pitch increase when going into a dive!)
            m_EngineSoundSource.pitch += m_Plane.ForwardSpeed*m_EngineFwdSpeedMultiplier;

            // Set the engine's volume to be proportional to the engine's current power.
            m_EngineSoundSource.volume = Mathf.InverseLerp(0, m_Plane.MaxEnginePower*m_AdvancedSetttings.engineMasterVolume,
                                                         m_Plane.EnginePower);

            // Set the wind's pitch and volume to be proportional to the aeroplane's forward speed.
            float planeSpeed = m_Rigidbody.velocity.magnitude;
            m_WindSoundSource.pitch = m_WindBasePitch + planeSpeed*m_WindSpeedPitchFactor;
            m_WindSoundSource.volume = Mathf.InverseLerp(0, m_WindMaxSpeedVolume, planeSpeed)*m_AdvancedSetttings.windMasterVolume;
        }
    }
}
