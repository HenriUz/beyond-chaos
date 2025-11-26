using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sound {
    public class SoundLibrary : MonoBehaviour {
        [SerializeField] private SoundEffectGroup[] soundEffectGroups;
        private Dictionary<string, AudioClip> sounds;

        private void Awake() {
            InitializeDictionary();
        }

        private void InitializeDictionary() {
            sounds = new Dictionary<string, AudioClip>();
            foreach (var soundEffectGroup in soundEffectGroups) {
                sounds[soundEffectGroup.name] = soundEffectGroup.clip;
            }
        }

        public AudioClip GetClip(string soundName) {
            return sounds[soundName];
        }
    }
}

[Serializable]
public struct SoundEffectGroup {
    public string name;
    public AudioClip clip;
}