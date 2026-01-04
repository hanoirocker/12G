namespace TwelveG.InteractableObjects
{
  using System.Collections;
  using Cinemachine;
  using TwelveG.GameController;
  using TwelveG.Localization;
  using TwelveG.PlayerController;
  using TwelveG.Utils;
  using UnityEngine;

  public class FlashlightPrefabHandler : MonoBehaviour, IInteractable
  {
    [Header("Settings")]
    [SerializeField, Range(2, 4)] private int cameraTrasitionTime = 3;

    [Header("Interaction Texts SO")]
    [SerializeField] private InteractionTextSO interactionTextsSO;

    [Header("References")]
    [SerializeField] private GameObject pickableObject;
    [SerializeField] private GameObject parentObject;

    private bool vcAnimationHasEnded = false;
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
      if (canBeInteractedWith)
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
      StartCoroutine(FlashlightInteraction());
      return true;
    }

    private IEnumerator FlashlightInteraction()
    {
      canBeInteractedWith = false;
      gameObject.GetComponent<Collider>().enabled = false;

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
      GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, 3));
      GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Flashlight, true));
      yield return new WaitForSeconds(3f);

      GameEvents.Common.playFlashlightVCAnimation.Raise(this, null);

      // TODO: Reproducir sonido de jugador agachandose

      yield return new WaitUntil(() => vcAnimationHasEnded);

      yield return new WaitForSeconds(1f);

      pickableObject.SetActive(true);
      pickableObject.GetComponent<PickableItem>().canBePicked = true;

      // Esperar hasta que el jugador levante tome el teléfono y lo agregue
      // al inventario.
      yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

      parentObject.GetComponent<Renderer>().enabled = false;

      // Vuelve a cámara de jugador
      GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, cameraTrasitionTime));
      GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Flashlight, false));
      yield return new WaitForSeconds(cameraTrasitionTime - 0.25f);
      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));

      parentObject.SetActive(false);
    }

    public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCameraObject)
    {
      throw new System.NotImplementedException();
    }

    public void OnAnimationHasEnded()
    {
      vcAnimationHasEnded = true;
    }
  }
}