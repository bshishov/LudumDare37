using System;
using System.Collections;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class Indicator : MonoBehaviour
    {
        public const string PLayerPositionXPrefsKey = "RelativeToRoomPositionX";
        public const string PLayerPositionYPrefsKey = "RelativeToRoomPositionY";
        public const string PLayerPositionZPrefsKey = "RelativeToRoomPositionZ";

        [Serializable]
        public struct LevelColor
        {
            public string Level;
            public Color ColorRequired;
        }

        public Vector3 PlayerRelPos
        {
            get
            {
                return new Vector3(PlayerPrefs.GetFloat(PLayerPositionXPrefsKey), 
                    PlayerPrefs.GetFloat(PLayerPositionYPrefsKey), 
                    PlayerPrefs.GetFloat(PLayerPositionZPrefsKey));
            }
            set
            {
                PlayerPrefs.SetFloat(PLayerPositionXPrefsKey, value.x);
                PlayerPrefs.SetFloat(PLayerPositionYPrefsKey, value.y);
                PlayerPrefs.SetFloat(PLayerPositionZPrefsKey, value.z);
            }
        }

        public Quaternion PlayerRelRot
        {
            get
            {
                return new Quaternion(
                    PlayerPrefs.GetFloat("RelRotX"),
                    PlayerPrefs.GetFloat("RelRotY"),
                    PlayerPrefs.GetFloat("RelRotZ"),
                    PlayerPrefs.GetFloat("RelRotW"));
            }
            set
            {
                PlayerPrefs.SetFloat("RelRotX", value.x);
                PlayerPrefs.SetFloat("RelRotY", value.y);
                PlayerPrefs.SetFloat("RelRotZ", value.z);
                PlayerPrefs.SetFloat("RelRotW", value.w);
            }
        }

        public Socket SourceSocket;
        public Door Door;
        public float RotationSpeed = 1000f;
        public float TestTime = 2f;
        public LevelColor[] LevelColors;
        public AudioClipWithVolume ActivateSound;

        private Rotator _rotator;
        private Transform _roomTransform;
        private Transform _playerTransform;
        private AudioSource _audioSource;
        private bool _loadStarted;

        void Start ()
        {
            _rotator = GetComponentInChildren<Rotator>();
            _roomTransform = GameObject.Find("Room").transform;
            _playerTransform = GameObject.FindGameObjectWithTag(Tags.Player).transform;
            _audioSource = GetComponent<AudioSource>();

            var p = PlayerRelPos;
            if (p.magnitude > 0f && SceneManager.GetActiveScene().name != "limbo")
            {
                _playerTransform.position = _roomTransform.position + PlayerRelPos;
                _playerTransform.localRotation = PlayerRelRot;
            }
        }
	
        void Update ()
        {
            if (!_loadStarted && SourceSocket.IsCharged && SourceSocket.Slot != null)
            {
                StartCoroutine(TryLoadLevel());
                _loadStarted = true;
            }
        }

        private IEnumerator TryLoadLevel()
        {
            if (Door.State == Door.DoorState.Opened)
            {
                Door.Close();
                Door.Locked = true;
            }
            SourceSocket.LockSlot();

            if(ActivateSound.Clip != null)
                _audioSource.PlayOneShot(ActivateSound.Clip, ActivateSound.VolumeModifier);


            _rotator.RotationSpeed = RotationSpeed;
            yield return new WaitForSeconds(TestTime);

            if (SourceSocket.Slot.ArtifactInSlot != null)
            {
                foreach (var levelColor in LevelColors)
                {
                    Debug.Log("Testing colors: " + SourceSocket.ConsumedColor + " vs " + levelColor.ColorRequired);
                    if (IdenticalColor(SourceSocket.ConsumedColor, levelColor.ColorRequired))
                    {
                        PlayerRelPos = _playerTransform.position - _roomTransform.position;
                        PlayerRelRot = _playerTransform.localRotation;
                        PlayerPrefs.Save();
                        SceneManager.LoadScene(levelColor.Level);
                    }
                }
            }

            SourceSocket.UnCharge();
            _rotator.RotationSpeed = 0;
            SourceSocket.UnLockSlot();
            _loadStarted = false;
            Door.Locked = false;
        }

        private bool IdenticalColor(Color a, Color b)
        {
            const float Threshold = 0.3f;
            if (Mathf.Abs(a.r - b.r) > Threshold)
                return false;

            if (Mathf.Abs(a.g - b.g) > Threshold)
                return false;

            if (Mathf.Abs(a.b - b.b) > Threshold)
                return false;

            return true;
        }
    }
}
