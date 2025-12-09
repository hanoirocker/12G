using System;
using System.Collections;
using TwelveG.AudioController;
using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public class CrashingBird : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 1000f;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip crashingBirdClip = null;
        [SerializeField, Range(0f, 1f)] private float crashingVolume = 0.8f;

        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            AddForce();
        }

        private void AddForce()
        {
            rb.AddForce(Vector3.right * moveSpeed);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Window")
            {
                StartCoroutine(PlayCrashSoundAndDestroy());
            }
        }

        private IEnumerator PlayCrashSoundAndDestroy()
        {
            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, crashingVolume);
            audioSource.clip = crashingBirdClip;
            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);
            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
        }
    }
}