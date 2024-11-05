namespace TwelveG.Environment
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CarsSpawner : MonoBehaviour
    {
        public List<GameObject> carPrefabs;
        public List<Material> carMaterials;
        [SerializeField] float timeTillRespawn = 0f;

        private float animationDuration = 0f;

        private void Start()
        {
            animationDuration = 0;
            StartCoroutine(CarCoroutine());
        }

        IEnumerator CarCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeTillRespawn + animationDuration);
                SpawnCar();
            }
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