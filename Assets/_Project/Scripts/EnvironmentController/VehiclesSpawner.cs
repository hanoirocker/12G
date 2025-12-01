namespace TwelveG.EnvironmentController
{
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.AudioController;
    using UnityEngine;

    public class VehiclesSpawner : MonoBehaviour
    {
        [Header("References")]
        [Space]
        public AnimationClip helicopterAnimationClip;
        public AudioClip helicopterSoundClip;
        public List<GameObject> carPrefabs;
        public List<Material> carMaterials;
        [Header("References")]
        [Space]
        [SerializeField] float timeTillRespawn = 0f;

        private float animationDuration = 0f;

        private void Start()
        {
            SpawnHelicopter();
        }

        IEnumerator CarCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeTillRespawn + animationDuration);
                SpawnCar();
            }
        }

        private void SpawnHelicopter()
        {
            float duration = helicopterAnimationClip.length;

            AudioManager.Instance.PoolsHandler.GetFreeTemporarySourceByType(
                AudioPoolType.Environment,
                duration,
                (source) =>
                {
                    source.clip = helicopterSoundClip;
                    source.rolloffMode = AudioRolloffMode.Logarithmic;
                    source.minDistance = 0.15f;
                    source.maxDistance = 200f;
                    source.spatialBlend = 1f;
                    source.Play();

                    Animation anim = source.gameObject.AddComponent<Animation>();
                    anim.AddClip(helicopterAnimationClip, helicopterAnimationClip.name);
                    anim.Play(helicopterAnimationClip.name);
                }
            );
        }

        private void SpawnCar()
        {
            GameObject selectedCarPrefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
            GameObject car = Instantiate(selectedCarPrefab, transform.position, transform.rotation, transform);

            Material randomMaterial = carMaterials[Random.Range(0, carMaterials.Count)];
            Renderer carRenderer = car.GetComponentInChildren<Renderer>();
            if (carRenderer != null)
            {
                carRenderer.material = randomMaterial;
            }
            animationDuration = 22f;
        }
    }

}