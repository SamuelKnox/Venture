using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

namespace Devdog.QuestSystemPro
{
    public partial class AudioManager : MonoBehaviour
    {
        public int reserveAudioSources = 8;
        public AudioMixerGroup audioMixerGroup;
//        public bool removeDuplicatesInSameFrame = true;

        private static AudioSource[] _audioSources;
        private static GameObject _audioSourceGameObject;

        private static List<AudioClipInfo> _audioQueueList = new List<AudioClipInfo>();


        protected virtual void Awake()
        {
            _audioSources = new AudioSource[reserveAudioSources];
            _audioQueueList = new List<AudioClipInfo>(_audioSources.Length);
            _audioSourceGameObject = new GameObject("QUEST_SYSTEM_AUDIO_SOURCES");
            _audioSourceGameObject.transform.SetParent(transform);

//            DontDestroyOnLoad(_audioSourceGameObject);

            for (int i = 0; i < _audioSources.Length; i++)
            {
                _audioSources[i] = _audioSourceGameObject.AddComponent<AudioSource>();
                _audioSources[i].outputAudioMixerGroup = audioMixerGroup;
            }
        }

        protected virtual void Update()
        {
            if (_audioQueueList.Count > 0)
            {
                var source = GetNextAudioSource();
                var clip = _audioQueueList[_audioQueueList.Count - 1];

                source.clip = clip.audioClip;
                source.pitch = clip.pitch;
                source.volume = clip.volume;
                source.loop = clip.loop;
                source.Play();

                _audioQueueList.RemoveAt(_audioQueueList.Count - 1);
            }
        }

        private static AudioSource GetNextAudioSource()
        {
            foreach (var audioSource in _audioSources)
            {
                if (audioSource.isPlaying == false)
                    return audioSource;
            }

            Debug.LogWarning("All sources taken, can't play audio clip...");
            return null;
        }


        /// <summary>
        /// Plays an audio clip, only use this for the UI, it is not pooled so performance isn't superb.
        /// </summary>
        public static void AudioPlayOneShot(AudioClipInfo clip)
        {
            if (clip == null || clip.audioClip == null)
                return;

#if UNITY_EDITOR
            if (FindObjectOfType<AudioManager>() == null)
            {
                QuestLogger.LogWarning("AudioManager component not found, yet trying to play an audio clip....");
            }
#endif

            _audioQueueList.Add(clip);
//            if (removeDuplicatesInSameFrame)
//            {
//                _audioQueueList = _audioQueueList.GroupBy(o => o.audioClip).Select(o => o.First()).ToList(); // Remove duplicates.
//            }
        }
    }
}