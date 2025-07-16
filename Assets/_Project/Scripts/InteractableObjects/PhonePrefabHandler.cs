namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
  using TwelveG.Utils;
  using UnityEngine;

    public class PhonePrefabHandler : MonoBehaviour, IInteractable
    {
        [Header("Prefab instances")]
        [SerializeField] private GameObject phoneParent;
        [SerializeField] private Animation phoneAnimation;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;
        [SerializeField] private ObservationTextSO observationTextSO;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onMainCameraSettings;
        [SerializeField] private GameEventSO onVirtualCamerasControl;
        [SerializeField] private GameEventSO onObservationCanvasShowText;
        [SerializeField] private GameEventSO onPlayerControls;

        [Header("References")]
        [SerializeField] private GameObject pickableObject;

        private bool playerIsInsideCollider = false;
        private bool canBeInteractedWith = true;

        public bool CanBeInteractedWith(PlayerInteraction playerCameraObject)
        {
            return true;
        }

        public ObservationTextSO GetFallBackText()
        {
            return null;
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            if (playerIsInsideCollider && canBeInteractedWith)
            {
                return interactionTextsSO;
            }
            else
            {
                return null;
            }
        }

        public bool Interact(PlayerInteraction playerCameraObject)
        {
            if (playerIsInsideCollider) { StartCoroutine(PhoneInteraction()); }

            return true;
        }

        private IEnumerator PhoneInteraction()
        {
            canBeInteractedWith = false;
            GetComponent<SphereCollider>().enabled = false;
            onPlayerControls.Raise(this, new TogglePlayerCapsule(false));

            onMainCameraSettings.Raise(this, "EasyInOut2");

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Phone, true));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Player, false));

            yield return new WaitForSeconds(2f);

            phoneAnimation.Play();

            yield return new WaitUntil(() => !phoneAnimation.isPlaying);

            yield return new WaitForSeconds(0.5f);
            onObservationCanvasShowText.Raise(this, observationTextSO);
            yield return new WaitForSeconds(0.5f);

            pickableObject.SetActive(true);
            pickableObject.GetComponent<PickableItem>().canBePicked = true;

            // Esperar hasta que el jugador levante tome el teléfono y lo agregue
            // al inventario.
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            GetComponentInParent<MeshRenderer>().enabled = false;

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Player, true));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Phone, false));

            yield return new WaitForSeconds(2.5f);
            onMainCameraSettings.Raise(this, "Cut");

            onPlayerControls.Raise(this, new TogglePlayerCapsule(true));

            Destroy(phoneParent);
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCameraObject)
        {
            throw new System.NotImplementedException();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerIsInsideCollider = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerIsInsideCollider = true;
            }
        }
    }
}