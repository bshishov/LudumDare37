using System;
using Assets.Scripts.Gameplay;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "Levels Data")]
    public class Levels : ScriptableObject
    {
        [Serializable]
        public struct LevelColor
        {
            public string Level;
            public Color ColorRequired;
        }

        public LevelColor[] LevelColors;
    }
}
