namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.Localization;
    using UnityEngine;

    public class PizzaSliceHandler : MonoBehaviour
    {
        [Header("Audio settings")]
        [SerializeField] private List<AudioClip> eatingAudios = new List<AudioClip>();

        [Header("Other eventsSO references")]
        public GameEventSO instantiatePoliceCar;
        public GameEventSO finishedEatingPizza;

        private AudioSource audioSource;
        private Animation animationComponent;

        private void Awake()
        {
            animationComponent = GetComponent<Animation>();
        }
        private void Start()
        {
            StartCoroutine(EatPizzaSlice());
        }

        private IEnumerator EatPizzaSlice()
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            animationComponent.PlayQueued("Pizza Slice - Idle");

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            animationComponent.PlayQueued("Pizza Slice - First Bite");
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            animationComponent.PlayQueued("Pizza Slice - Second Bite");
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            // Avisa a PizzaTimeEvent que debe instanciar el auto de policia.
            instantiatePoliceCar.Raise(this, null);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            animationComponent.PlayQueued("Pizza Slice - Third Bite");
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            // Avisa a PizzaTimeEvent que terminó de comer.
            finishedEatingPizza.Raise(this, null);

            yield return new WaitForSeconds(1f);

            Destroy(gameObject);
        }
    }
}