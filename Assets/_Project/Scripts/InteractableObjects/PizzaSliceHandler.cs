using System.Collections;
using System.Collections.Generic;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class PizzaSliceHandler : MonoBehaviour
    {
        [Header("Audio settings")]
        [SerializeField] private List<AudioClip> eatingAudios = new List<AudioClip>();

        [Header("Other eventsSO references")]
        public GameEventSO instantiatePoliceCar;
        public GameEventSO finishedEatingPizza;

        [Header("Text SO's")]
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;

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
            // Espera inicial (FadeIn + FadeOut de PizzaTimeEvent + 1 segundo extra)
            yield return new WaitForSeconds(3f);

            // Iniciamos Idle. Usamos Play para asegurar estado.
            animationComponent.Play("Pizza Slice - Idle");

            UIManager.Instance.InteractionCanvasHandler.ShowEventInteractionText(eventsInteractionTextsSO);

            // --- PRIMER MORDISCO ---
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            UIManager.Instance.InteractionCanvasHandler.HideInteractionText();

            // Ejecutamos la animación (Play y wait por duración es más seguro que isPlaying)
            PlayAnimationAndWait("Pizza Slice - First Bite");
            // Esperamos la duración del clip
            yield return new WaitForSeconds(animationComponent["Pizza Slice - First Bite"].length);


            UIManager.Instance.InteractionCanvasHandler.ShowEventInteractionText(eventsInteractionTextsSO);

            // --- SEGUNDO MORDISCO ---
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            UIManager.Instance.InteractionCanvasHandler.HideInteractionText();

            // Avisa a PizzaTimeEvent que debe instanciar el auto de policia.
            instantiatePoliceCar.Raise(this, null);

            animationComponent.Play("Pizza Slice - Second Bite");
            yield return new WaitForSeconds(animationComponent["Pizza Slice - Second Bite"].length);
            
            UIManager.Instance.InteractionCanvasHandler.ShowEventInteractionText(eventsInteractionTextsSO);

            // --- TERCER MORDISCO ---
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            UIManager.Instance.InteractionCanvasHandler.HideInteractionText();

            animationComponent.Play("Pizza Slice - Third Bite");
            yield return new WaitForSeconds(animationComponent["Pizza Slice - Third Bite"].length);

            // Avisa a PizzaTimeEvent que terminó de comer.
            finishedEatingPizza.Raise(this, null);
            Destroy(gameObject);
        }

        private void PlayAnimationAndWait(string clipName)
        {
            animationComponent.Play(clipName);
        }
    }
}