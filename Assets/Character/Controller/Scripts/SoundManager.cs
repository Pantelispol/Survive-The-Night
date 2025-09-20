using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SmallHedge.SoundManager
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private SoundsSO SO;
        private static SoundManager instance = null;
        private AudioSource audioSource;

        private void Awake()
        {
            if(!instance)
            {
                instance = this;
                audioSource = GetComponent<AudioSource>();
            }
        }

        public static void PlaySound(SoundType sound, Vector3 position, float volume = 1f)
        {
            SoundList soundList = instance.SO.sounds[(int)sound];
            AudioClip[] clips = soundList.sounds;
            AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

            GameObject tempGO = new GameObject("TempAudio"); 
            tempGO.transform.position = position; 

            AudioSource aSource = tempGO.AddComponent<AudioSource>(); 
            aSource.clip = randomClip;
            aSource.volume = volume * soundList.volume;
            aSource.outputAudioMixerGroup = soundList.mixer;
            aSource.spatialBlend = 1f;
            aSource.minDistance = 1f; 
            aSource.maxDistance = 20f;
            aSource.rolloffMode = AudioRolloffMode.Linear;

            aSource.Play();
            UnityEngine.Object.Destroy(tempGO, randomClip.length);
        }

    }

    [Serializable]
    public struct SoundList
    {
        [HideInInspector] public string name;
        [Range(0, 1)] public float volume;
        public AudioMixerGroup mixer;
        public AudioClip[] sounds;
    }
}