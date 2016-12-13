using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    [Serializable]
    public struct AudioClipWithVolume
    {
        public AudioClip Clip;

        [Range(0f, 1f)]
        public float VolumeModifier;
    }
}
