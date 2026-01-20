using System;
using UnityEngine;

namespace TwelveG.PlayerController
{
  public class PlayerSpotter : MonoBehaviour
  {
    [Header("Raycast settings")]
    [Tooltip("Capa de los enemigos/objetos (VisualSpotting)")]
    [SerializeField] private LayerMask targetMask;

    [Tooltip("Capa de las paredes/obstáculos (Default)")]
    [SerializeField] private LayerMask obstructionMask;

    [SerializeField, Range(0.5f, 50f)] private float spottingRange = 35f;

    [SerializeField, Range(0.1f, 2f)] private float detectionRadius = 0.5f;

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
      Spot();
    }

    private void Spot()
    {
      Ray r = new Ray(interactorSource.position, interactorSource.forward);
      ISpot potentialTarget = null;
      RaycastHit hitInfo;

      // SphereCast SOLO buscando objetivos (ignora paredes)
      if (Physics.SphereCast(r, detectionRadius, out hitInfo, spottingRange, targetMask))
      {
        // Encontramos al objetivo, pero puede estar siendo obstruido por una pared
        if (hitInfo.collider.gameObject.TryGetComponent(out ISpot spotteableObject))
        {
          if (spotteableObject.CanBeSpotted())
          {
            // Verificación de Línea de Visión (Line of Sight)
            // Trazamos una línea desde la cámara hasta el punto exacto del enemigo
            Vector3 targetPoint = hitInfo.collider.bounds.center;
            Vector3 directionToTarget = targetPoint - interactorSource.position;
            float distanceToTarget = directionToTarget.magnitude;

            // Si lanzamos un rayo hacia el enemigo y NO pegamos en una pared (obstructionMask)...
            if (!Physics.Raycast(interactorSource.position, directionToTarget, distanceToTarget, obstructionMask))
            {
              potentialTarget = spotteableObject;
            }
          }
        }
      }

      if (potentialTarget != null)
      {
        potentialTarget.SpotOnObject(cameraZoom.playerIsZooming());

        if (lastSpottedObject != null && lastSpottedObject != potentialTarget)
        {
          lastSpottedObject.IsAbleToBeSpotted(true);
        }

        lastSpottedObject = potentialTarget;
      }
      else
      {
        // Si antes estaba mirando algo, le aviso que ya no lo miro
        if (lastSpottedObject != null)
        {
          lastSpottedObject.IsAbleToBeSpotted(true);
        }

        lastSpottedObject = null;
      }
    }

    public ISpot GetCurrentlySpottedObject()
    {
      // Solo devolvemos el objeto si NO es nulo (es decir, si lo estamos mirando ahora)
      return lastSpottedObject;
    }

    private void OnDrawGizmos()
    {
      Gizmos.color = raycastColor;
      Gizmos.DrawRay(interactorSource.position, interactorSource.forward * spottingRange);
      Gizmos.DrawWireSphere(interactorSource.position + interactorSource.forward * spottingRange, detectionRadius);
    }
  }
}