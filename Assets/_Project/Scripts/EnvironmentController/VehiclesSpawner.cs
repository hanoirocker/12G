namespace TwelveG.EnvironmentController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum VehicleType
    {
        SlowCars,
        FastCars,
        Helicopter1,
        RegularPoliceCar,
        PoliceCarCrash,
    }

    public class VehiclesSpawner : MonoBehaviour
    {
        [SerializeField] private Transform vehiclesParent;
        [Space]
        [SerializeField] private GameObject helicopterPrefb;
        [Header("Regular Cars References")]
        [Space]
        [SerializeField] private List<Color> carColors = new List<Color>();
        [SerializeField] private List<AudioClip> regularCarsSoundClip;
        [SerializeField] private List<GameObject> carPrefabs;
        [SerializeField] private List<AnimationClip> regularCarsAnimationClips;
        [Header("Police Cars References")]
        [Space]
        [SerializeField] private GameObject policeCar1;
        [SerializeField] private GameObject poliecCar2;

        private bool vehicleInScene = false;

        private IEnumerator SpawnHelicopterCoroutine()
        {
            yield return StartCoroutine(VerifyVehiclesInScene());

            GameObject activeHelicopter = Instantiate(helicopterPrefb, vehiclesParent);
            AudioSource audioSource = activeHelicopter.GetComponent<AudioSource>();
            Animation animationComponent = activeHelicopter.GetComponent<Animation>();

            yield return new WaitUntil(() => !animationComponent.isPlaying);
            audioSource.Stop();
            Destroy(activeHelicopter);
        }

        private IEnumerator SpawnCarCoroutine(float speedMod)
        {
            yield return StartCoroutine(VerifyVehiclesInScene());

            vehicleInScene = true;

            int carIndex = Random.Range(0, carPrefabs.Count);
            int animationIndex = Random.Range(0, regularCarsAnimationClips.Count);
            int soundIndex = Random.Range(0, regularCarsSoundClip.Count);

            GameObject activeCar = carPrefabs[carIndex];
            // Resetear rotacion y posicion
            activeCar.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            // Configuracion de animacion
            Animation animationComponent = activeCar.GetComponent<Animation>();
            animationComponent.AddClip(
                regularCarsAnimationClips[animationIndex],
                regularCarsAnimationClips[animationIndex].name
            );
            animationComponent[regularCarsAnimationClips[animationIndex].name].speed = speedMod;

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
            AudioSource audioSource = activeCar.GetComponent<AudioSource>();
            audioSource.clip = regularCarsSoundClip[soundIndex];

            activeCar.SetActive(true);

            animationComponent.Play(regularCarsAnimationClips[animationIndex].name);
            audioSource.Play();
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            // Reset de audio source
            audioSource.Stop();

            activeCar.SetActive(false);
            vehicleInScene = false;
        }

        private IEnumerator SpawnPoliceCar1Routine()
        {
            yield return StartCoroutine(VerifyVehiclesInScene());

            vehicleInScene = true;
            GameObject activePoliceCar = Instantiate(policeCar1, vehiclesParent);

            Animation animationComponent = activePoliceCar.GetComponent<Animation>();
            AudioSource audioSource = activePoliceCar.GetComponent<AudioSource>();

            yield return new WaitUntil(() => !animationComponent.isPlaying);
            audioSource.Stop();
            Destroy(activePoliceCar);
            vehicleInScene = false;
        }

        // La logica de la explosion está en el prefab
        private IEnumerator SpawnPoliceCar2Routine()
        {
            yield return StartCoroutine(VerifyVehiclesInScene());

            vehicleInScene = true;
            GameObject activePoliceCar = Instantiate(poliecCar2, vehiclesParent);

            Animation animationComponent = activePoliceCar.GetComponent<Animation>();
            AudioSource audioSource = activePoliceCar.GetComponent<AudioSource>();

            yield return new WaitUntil(() => !animationComponent.isPlaying);
            vehicleInScene = false;
        }

        private IEnumerator VerifyVehiclesInScene()
        {
            if (vehicleInScene)
            {
                Debug.Log($"Already vehicle spawned, waiting .. ");
                yield return new WaitUntil(() => !vehicleInScene);
            }
            else
            {
                yield break;
            }
        }

        public void PauseGame(Component sender, object data)
        {
            {
                bool pause = (bool)data;
                AudioSource[] audioSources = vehiclesParent.GetComponentsInChildren<AudioSource>();
                foreach (AudioSource source in audioSources)
                {
                    if (pause)
                    {
                        source.Pause();
                    }
                    else
                    {
                        source.UnPause();
                    }
                }
            }
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
                        StartCoroutine(SpawnCarCoroutine(Random.Range(1.5f, 1.8f)));
                        break;
                    case VehicleType.RegularPoliceCar:
                        StartCoroutine(SpawnPoliceCar1Routine());
                        break;
                    case VehicleType.PoliceCarCrash:
                        StartCoroutine(SpawnPoliceCar2Routine());
                        break;
                    case VehicleType.Helicopter1:
                        StartCoroutine(SpawnHelicopterCoroutine());
                        break;
                }
            }
        }
    }

}