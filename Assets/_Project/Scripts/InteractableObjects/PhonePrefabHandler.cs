using System.Collections;
using Cinemachine;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
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

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            return (null, 0f);
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
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
            gameObject.GetComponent<SphereCollider>().enabled = false;

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, cameraTrasitionTime));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Phone, true));
            yield return new WaitForSeconds(cameraTrasitionTime);

            phoneAnimation.Play();

            yield return new WaitUntil(() => !phoneAnimation.isPlaying);

            yield return new WaitForSeconds(0.5f);
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(observationTextSO);
            yield return new WaitForSeconds(0.5f);

            pickableObject.SetActive(true);
            pickableObject.GetComponent<PickableItem>().canBePicked = true;

            // Esperar hasta que el jugador levante tome el teléfono y lo agregue
            // al inventario.
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            GetComponentInParent<MeshRenderer>().enabled = false;

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

            // Cambia a Camera del Sofá
            GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, cameraTrasitionTime));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Sofa, true));
            yield return new WaitForSeconds(cameraTrasitionTime - 0.5f);

            Destroy(phoneParent);
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCameraObject)
        {
            throw new System.NotImplementedException();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PlayerCapsule"))
            {
                playerIsInsideCollider = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("PlayerCapsule"))
            {
                playerIsInsideCollider = true;
            }
        }
    }
}