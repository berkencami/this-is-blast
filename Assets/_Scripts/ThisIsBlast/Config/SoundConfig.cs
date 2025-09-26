using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThisIsBlast.Config
{
    [CreateAssetMenu(menuName = "ScriptableObjects/SoundConfig", fileName = "SoundConfig")]
    public class SoundConfig : ScriptableObject
    {
        public List<SoundFX> soundFx;
    }

    [Serializable]
    public class SoundFX
    {
        public SoundType soundType;
        public AudioClip audioClip;
        public float volume;
    }

    public enum SoundType
    {
        Shoot = 0,
        ShooterClick = 1,
        LevelSuccess = 2,
        LevelFail = 3,
    }
}