namespace TwelveG.Environment
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PoliceCar : MonoBehaviour
    {
        [SerializeField] List<Light> carLights = new List<Light>();
        [SerializeField] private float lightToggleTime = 0.1f;
        private Animation animationComponent;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            animationComponent = GetComponent<Animation>();
        }

        private void Start()
        {
            StartCoroutine(PoliceCarCorutine());
        }

        private IEnumerator PoliceCarCorutine()
        {
            audioSource.Play();
            while (animationComponent.isPlaying)
            {
                foreach (Light light in carLights)
                {
                    light.enabled = !light.enabled;
                }
                yield return new WaitForSeconds(lightToggleTime);
            }
            audioSource.Stop();
            Destroy(gameObject);
        }
    }
}