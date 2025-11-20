using System;
using UnityEngine;
namespace TwelveG.PlayerController
{
  public class PlayerSpotter : MonoBehaviour
  {
    [Header("Raycast settings")]
    [SerializeField] private LayerMask verificableMask;
    [SerializeField, Range(0.5f, 50f)] private float verificationRange = 35f;
    public Color raycastColor;

    [Header("Cameras Settings")]
    [SerializeField] private Transform interactorSource;

    private CameraZoom cameraZoom;
    private ISpot lastSpottedObject = null;

    private void Awake()
    {
      cameraZoom = GetComponent<CameraZoom>();
    }

    private void Update()
    {
      if (cameraZoom.playerIsZooming())
      {
        CheckOnObject();
      }
      else
      {
        return;
      }
    }

    private void CheckOnObject()
    {
      Ray r = new Ray(interactorSource.position, interactorSource.forward);

      if (Physics.Raycast(r, out RaycastHit hitInfo, verificationRange, verificableMask))
      {
        bool hasVerificable = hitInfo.collider.gameObject.TryGetComponent(out ISpot spotteableObject);
        Debug.Log($"Object {hitInfo.collider.gameObject.name} hasVerificable: {hasVerificable}");
        if (hasVerificable && spotteableObject.CanBeSpotted())
        {
          Debug.Log("hasVerificable && spotteableObject.CanBeChecked()");
          // Si estoy contemplando un objeto distinto al anterior
          if (lastSpottedObject != null && lastSpottedObject != spotteableObject)
          {
            lastSpottedObject.IsAbleToBeSpotted(true);
          }

          lastSpottedObject = spotteableObject;
        }
      }
      else
      {
        // Si no golpeamos nada, re-habilitamos el último contemplado
        if (lastSpottedObject != null)
        {
          lastSpottedObject.IsAbleToBeSpotted(true);
          lastSpottedObject = null;
        }
      }
    }

    private void OnDrawGizmos()
    {

      Gizmos.color = raycastColor;
      Vector3 direction = interactorSource.TransformDirection(Vector3.forward) * verificationRange;
      Gizmos.DrawRay(interactorSource.position, direction);
    }
  }
}
