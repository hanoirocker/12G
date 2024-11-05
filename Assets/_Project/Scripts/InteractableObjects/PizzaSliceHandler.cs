namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PizzaSliceHandler : MonoBehaviour
    {
        //TODO: LOCALIZATION! eventtextsSO
        [Header("Event options texts")]
        [SerializeField] private string defaultEventControlOptions;

        [Header("Objects options")]
        [SerializeField] private GameObject policeCar;

        [Header("Audio settings")]
        [SerializeField] private List<AudioClip> eatingAudios = new List<AudioClip>();
        [SerializeField] private AudioClip chairMovingSound = null;

        [Header("EventsSO references")]
        public GameEventSO onControlCanvasSetInteractionOptions;
        public GameEventSO onControlCanvasControls;
        public GameEventSO onImageCanvasControls;
        public GameEventSO onPlayerControls;
        public GameEventSO onVirtualCamerasControl;
        public GameEventSO onInteractionCanvasShowText;
        public GameEventSO onInteractionCanvasControls;

        [Header("Other eventsSO references")]
        public GameEventSO finishedEatingPizza;

        private AudioSource audioSource;
        private Animation animationComponent;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            animationComponent = GetComponent<Animation>();
        }
        private void Start()
        {
            StartCoroutine(EatPizzaSlice());
        }

        private IEnumerator EatPizzaSlice()
        {
            onImageCanvasControls.Raise(this, "FadeOutImage");
            yield return new WaitForSeconds(1f);

            onPlayerControls.Raise(this, "DisablePlayerCapsule");

            onVirtualCamerasControl.Raise(this, "EnableKitchenDeskVC");

            onPlayerControls.Raise(this, "EnableHeadLookAround");

            if (chairMovingSound)
            {
                audioSource.PlayOneShot(chairMovingSound);
                yield return new WaitUntil(() => !audioSource.isPlaying);
            }

            onImageCanvasControls.Raise(this, "FadeInImage");
            yield return new WaitForSeconds(1f);

            onControlCanvasControls.Raise(this, "ShowControlCanvas");

            onControlCanvasSetInteractionOptions.Raise(this, defaultEventControlOptions);

            animationComponent.PlayQueued("Pizza Slice - Idle");

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            animationComponent.PlayQueued("Pizza Slice - First Bite");
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            animationComponent.PlayQueued("Pizza Slice - Second Bite");
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            Instantiate(policeCar);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            animationComponent.PlayQueued("Pizza Slice - Third Bite");
            yield return new WaitUntil(() => !animationComponent.isPlaying);

            onControlCanvasControls.Raise(this, "ResetEventControlsOptions");
            yield return new WaitForSeconds(2f);

            onInteractionCanvasShowText.Raise(this, "LEVANTARSE [E]");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            onImageCanvasControls.Raise(this, "FadeOutImage2");
            yield return new WaitForSeconds(2f);

            onPlayerControls.Raise(this, "EnablePlayerCapsule");

            onVirtualCamerasControl.Raise(this, "DisableKitchenDeskVC");

            onPlayerControls.Raise(this, "DisableHeadLookAround");

            onInteractionCanvasControls.Raise(this, "HideText");

            onImageCanvasControls.Raise(this, "FadeInImage2");
            yield return new WaitForSeconds(2f);

            // Avisa a PizzaTimeEvent que terminó de comer.
            finishedEatingPizza.Raise(this, null);

            yield return new WaitForSeconds(1f);

            Destroy(gameObject);
        }
    }
}