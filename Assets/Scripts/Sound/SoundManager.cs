using UnityEngine;

namespace Sound {
    public class SoundManager : MonoBehaviour {
        private static SoundManager Instance { get; set; }

        private static AudioSource _backgroundAudioSource;
        private static AudioSource _voiceAudioSource;
        private static SoundLibrary _soundLibrary;
        
        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            var audioSources = GetComponents<AudioSource>();
            _backgroundAudioSource = audioSources[0];
            _voiceAudioSource = audioSources[1];
            
            _soundLibrary = GetComponent<SoundLibrary>();
        }

        public static void PlayBackground(string soundName) {
            if (soundName == null) {
                _backgroundAudioSource.Stop();
                return;
            }
            
            var audioClip = _soundLibrary.GetClip(soundName);
            if (audioClip == null) return;
            
            _backgroundAudioSource.clip = audioClip;
            _backgroundAudioSource.Play();
        }

        public static void PlayVoice(AudioClip audioClip, float pitch = 1f) {
            _voiceAudioSource.pitch = pitch;
            _voiceAudioSource.PlayOneShot(audioClip);
        }
        
        public static void SetVolume(float volume) {
            _backgroundAudioSource.volume = volume;
            _voiceAudioSource.volume = volume;
        }
    }
}