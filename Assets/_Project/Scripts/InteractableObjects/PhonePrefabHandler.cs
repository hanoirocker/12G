namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using Cinemachine;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using TwelveG.Utils;
    using UnityEngine;

    public class PhonePrefabHandler : MonoBehaviour, IInteractable
    {
        [Header("Settings")]
        [SerializeField, Range(2, 4)] private int cameraTrasitionTime = 2;

        [Header("Prefab Instances")]
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
            onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, cameraTrasitionTime));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Phone, true));
            yield return new WaitForSeconds(cameraTrasitionTime);

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

            onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, cameraTrasitionTime));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Sofa, true));
            yield return new WaitForSeconds(cameraTrasitionTime - 0.5f);

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