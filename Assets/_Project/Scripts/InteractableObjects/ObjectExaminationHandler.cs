namespace TwelveG.InteractableObjects
{
  using TwelveG.Localization;
  using TwelveG.PlayerController;
  using UnityEngine;

  public class ObjectExaminationHandler : MonoBehaviour, IInteractable
  {
    [Header("References")]
    [SerializeField] private GameObject examinablePrefab;

    [Header("Interaction Texts SO's")]
    [SerializeField] private InteractionTextSO interactionTextsSO;

    [Header("EventsSO references")]
    [SerializeField] private GameEventSO onObjectExaminationStart;

    private bool canBeExamined = true;

    public bool CanBeInteractedWith(PlayerInteraction playerCameraObject)
    {
      return canBeExamined;
    }

    public InteractionTextSO RetrieveInteractionSO()
    {
      return interactionTextsSO;
    }

    public bool Interact(PlayerInteraction playerCameraObject)
    {
      if (canBeExamined)
      {
        onObjectExaminationStart.Raise(this, examinablePrefab);
        return true;
      }
      else
      {
        return true;
      }
    }

    public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCameraObject)
    {
      throw new System.NotImplementedException();
    }

    public ObservationTextSO GetFallBackText()
    {
      return null;
    }
  }
}