using System;
using System.Collections;
using Assets.Scripts.Data;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class Indicator : MonoBehaviour
    {
        public const string PlayerSavePosKey = "PlayerPosInRoom";
        public const string PlayerSaveRotKey = "PlayerRotInRoom";

        public Vector3 PlayerRelPos
        {
            get { return Utilities.GetPlayerPrefsVector(PlayerSavePosKey); }
            set { Utilities.SetPlayerPrefsVector(PlayerSavePosKey, value); }
        }

        public Quaternion PlayerRelRot
        {
            get { return Utilities.GetPlayerPrefsQuaternion(PlayerSaveRotKey); }
            set { Utilities.SetPlayerPrefsQuaternion(PlayerSaveRotKey, value); }
        }

        public Socket SourceSocket;
        public Door Door;
        public float RotationSpeed = 1000f;
        public float TestTime = 2f;
        public Levels Levels;
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

            if (SourceSocket.Slot.ArtifactInSlot != null && Levels != null)
            {
                foreach (var levelColor in Levels.LevelColors)
                {
                    Debug.Log("Testing colors: " + SourceSocket.ConsumedColor + " vs " + levelColor.ColorRequired);
                    if (Utilities.IsIdenticalColor(SourceSocket.ConsumedColor, levelColor.ColorRequired, 0.2f))
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
    }
}
