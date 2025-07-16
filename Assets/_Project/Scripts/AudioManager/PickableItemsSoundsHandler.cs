namespace TwelveG.AudioManager
{
    using UnityEngine;

    public class PickableItemsSoundsHandler : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayPickableItemSound(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}