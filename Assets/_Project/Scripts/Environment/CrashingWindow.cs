namespace TwelveG.Environment
{
    using System.Collections;
    using UnityEngine;

    public class CrashingWindow : MonoBehaviour
    {
        private AudioSource audioSource;

        public void PlayCrashingWindowSound()
        {
            StartCoroutine(PlayCrashingWindowSoundCoroutine());
        }

        private IEnumerator PlayCrashingWindowSoundCoroutine()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }
    }
}