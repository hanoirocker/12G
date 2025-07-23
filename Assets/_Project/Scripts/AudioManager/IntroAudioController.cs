namespace TwelveG.AudioController
{
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(GameEventListener))]
    public class IntroAudioController : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public IEnumerator AudioFadeInSequence()
        {
            yield return StartCoroutine(RunAudioFadeIn(0f, 0.5f, 10f));
        }

        public IEnumerator RunAudioFadeIn(float from, float to, float duration)
        {
            audioSource.Play();

            float elapsed = 0f;

            while (elapsed < duration)
            {
                audioSource.volume = Mathf.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            audioSource.volume = to;
        }

        public void RunAudioFadeOut(Component sender, object data)
        {
            if (data != null)
            {
                StartCoroutine(AudioFadeOutSequence((float)data));
            }
        }

        private IEnumerator AudioFadeOutSequence(float duration)
        {
            if (audioSource == null || !audioSource.isPlaying)
                yield break;

            float elapsed = 0f;
            float startVolume = audioSource.volume;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            audioSource.volume = 0f;
            audioSource.Stop();
        }
    }
}