namespace TwelveG.EnvironmentController
{
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.AudioController;
    using UnityEngine;

    public enum VehicleType
    {
        SlowCars,
        FastCars,
        Helicopter1,
        PoliceCarWithCrash,
        PoliceCarWithSiren
    }

    public class VehiclesSpawner : MonoBehaviour
    {
        [Header("Colors")]
        [Space]
        [SerializeField] private List<Color> carColors = new List<Color>();
        [Header("References")]
        [Space]
        public Transform vehiclesParent;
        [Space]
        public AnimationClip helicopterAnimationClip;
        public AudioClip helicopterSoundClip;
        [Space]
        public List<AudioClip> regularCarsSoundClip;
        public List<GameObject> carPrefabs;
        public List<AnimationClip> regularCarsAnimationClips;

        private AudioSource audioSource;

        private IEnumerator SpawnHelicopterCoroutine()
        {
            audioSource = vehiclesParent.GetComponent<AudioSource>();
            var originalState = audioSource.GetSnapshot();
            audioSource.clip = helicopterSoundClip;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.minDistance = 0.4f;
            audioSource.maxDistance = 250f;
            audioSource.spatialBlend = 1f;

            Animation animationComponenet = vehiclesParent.GetComponent<Animation>();
            animationComponenet.AddClip(helicopterAnimationClip, helicopterAnimationClip.name);
            audioSource.Play();
            animationComponenet.Play(helicopterAnimationClip.name);

            yield return new WaitForSeconds(helicopterAnimationClip.length);
            audioSource.Stop();
            audioSource.RestoreSnapshot(originalState);
        }

        private IEnumerator SpawnCarCoroutine(float speedMod)
        {
            int carIndex = Random.Range(0, carPrefabs.Count);
            int animationIndex = Random.Range(0, regularCarsAnimationClips.Count);
            int soundIndex = Random.Range(0, regularCarsSoundClip.Count);

            GameObject activeCar = Instantiate(carPrefabs[carIndex], vehiclesParent);

            // Configuracion de animacion
            Animation animationComponenet = vehiclesParent.GetComponent<Animation>();
            animationComponenet.AddClip(
                regularCarsAnimationClips[animationIndex],
                regularCarsAnimationClips[animationIndex].name
            );
            animationComponenet[regularCarsAnimationClips[animationIndex].name].speed = speedMod;

            // Obtener material, copiarlo y modificarlo si la lista de Colores no esta vacia
            if (carColors != null && carColors.Count > 0)
            {
                Color chosenColor = carColors[Random.Range(0, carColors.Count)];
                Renderer carRenderer = activeCar.GetComponent<Renderer>();
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                carRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_BaseColor", chosenColor);
                carRenderer.SetPropertyBlock(propertyBlock);
            }


            // Configuracion de audio
            audioSource = vehiclesParent.GetComponent<AudioSource>();
            var originalState = audioSource.GetSnapshot();
            audioSource.clip = regularCarsSoundClip[soundIndex];
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.pitch = speedMod * 0.5f;
            audioSource.minDistance = 0.2f;
            audioSource.maxDistance = 80f;
            audioSource.spatialBlend = 1f;

            animationComponenet.Play(regularCarsAnimationClips[animationIndex].name);
            audioSource.Play();
            yield return new WaitForSeconds(regularCarsAnimationClips[animationIndex].length);

            // Reset de audio source
            audioSource.Stop();
            audioSource.RestoreSnapshot(originalState);

            activeCar.SetActive(false);
        }

        public void SpawnVehicle(Component sender, object data)
        {
            if (data != null)
            {
                switch ((VehicleType)data)
                {
                    case VehicleType.SlowCars:
                        StartCoroutine(SpawnCarCoroutine(Random.Range(1f, 1.4f)));
                        break;
                    case VehicleType.FastCars:
                        StartCoroutine(SpawnCarCoroutine(Random.Range(1.4f, 1.8f)));
                        break;
                    case VehicleType.PoliceCarWithSiren:
                        // TODO
                        break;
                    case VehicleType.PoliceCarWithCrash:
                        // TODO
                        break;
                    case VehicleType.Helicopter1:
                        StartCoroutine(SpawnHelicopterCoroutine());
                        break;
                }
            }
        }
    }

}