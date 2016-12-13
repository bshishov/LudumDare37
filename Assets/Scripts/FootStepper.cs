using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Utility;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CustomCharacterController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class FootStepper : MonoBehaviour
{
    [Serializable]
    public struct StepsAudioClipSet
    {
        public AudioClip[] FootStepClips;
        public AudioClip JumpClip;
        public AudioClip LandClip;

        [Range(0f, 1f)]
        public float VolumeScale;
    }

    public StepsAudioClipSet Concrete;
    public StepsAudioClipSet Dirt;
    public AudioClipWithVolume WindSpeed;
    public float DistanceBetweenSteps = 100f;
    public float DistanceBetweenStepsRun = 100f;
    [Range(0f, 1f)]
    public float MasterVolumeScale = 1f;

    private CustomCharacterController _characterController;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private float _currentDistance;
    private StepsAudioClipSet _current;

    void Start ()
	{
	    _rigidbody = GetComponent<Rigidbody>();
	    _characterController = GetComponent<CustomCharacterController>();
        _audioSource = GetComponent<AudioSource>();
        _currentDistance = 0f;
        _current = Concrete;

	}
	
	void Update ()
    {
	    if (_characterController.Grounded)
	    {
	        if (_audioSource.isPlaying)
	        {
	            _audioSource.clip = null;
	            _audioSource.volume = 1f;
	        }

	        _currentDistance += _rigidbody.velocity.magnitude;
	        if (_characterController.Running)
	        {
                if (_currentDistance >= DistanceBetweenStepsRun)
                {
                    _audioSource.PlayOneShot(GetRandomClip(), MasterVolumeScale * _current.VolumeScale);
                    _currentDistance = 0f;
                }
            }
	        else
	        {
                if (_currentDistance >= DistanceBetweenSteps)
                {
                    _audioSource.PlayOneShot(GetRandomClip(), MasterVolumeScale * _current.VolumeScale);
                    _currentDistance = 0f;
                }
            }
	    }
	    else
	    {
	        if (WindSpeed.Clip != null)
	        {
	            if (_audioSource.clip != WindSpeed.Clip)
	            {
	                _audioSource.clip = WindSpeed.Clip;
	                _audioSource.Play();
	            }
	            _audioSource.volume = Mathf.Clamp01(_rigidbody.velocity.magnitude * 0.05f);
	        }
	    }
	}

    AudioClip GetRandomClip()
    {
        return _current.FootStepClips[Random.Range(0, _current.FootStepClips.Length)];
    }

    void OnJump()
    {
        if (_current.JumpClip != null)
            _audioSource.PlayOneShot(_current.JumpClip, MasterVolumeScale * _current.VolumeScale);
    }

    void OnLand()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.clip = null;
            _audioSource.volume = 1f;
        }
        if (_current.LandClip != null)
            _audioSource.PlayOneShot(_current.LandClip, MasterVolumeScale * _current.VolumeScale);
    }
}
