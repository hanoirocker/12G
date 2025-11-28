using System.Collections.Generic;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
  public class ObjectExaminationHandler : MonoBehaviour, IInteractable
  {
    [Header("References")]
    [SerializeField] private GameObject examinablePrefab;
    [SerializeField] private List<MeshRenderer> meshesToHide;

    [Header("Interaction Texts SO's")]
    [SerializeField] private InteractionTextSO interactionTextsSO;

    [Header("EventsSO references")]
    [SerializeField] private GameEventSO onObjectExaminationStart;

    private bool canBeExamined = true;

    public void ShowObjectInScene(bool show)
    {
      foreach (var mesh in meshesToHide)
      {
        mesh.enabled = show;
      }
    }

    public bool CanBeInteractedWith(PlayerInteraction playerCameraObject)
    {
      return canBeExamined;
    }

    public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
    {
      return interactionTextsSO;
    }

    public bool Interact(PlayerInteraction playerCameraObject)
    {
      if (canBeExamined)
      {
        // Avisame al MainCameraHandler que debe instanciar el prefab examinable
        onObjectExaminationStart.Raise(this, examinablePrefab);
        // Desactiva los meshes del object
        ShowObjectInScene(false);
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