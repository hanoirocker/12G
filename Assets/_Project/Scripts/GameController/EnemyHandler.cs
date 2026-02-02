using System.Collections;
using TwelveG.AudioController;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.GameController
{
  public enum EnemyAnimations
  {
    VisionsOutsideDhallToEntrance,
    VoicesMainGarageToEntrance,
    VoicesGarageToDhall,
    VoicesDhallToKichen,
    VoicesKichenToKichenDepot,
    VoicesKichenDepotToKichenTable,
    VoicesKitchenTableToGarage
  }

  public enum EnemyPositions
  {
    None,
    PlayerHouseCorner,
    MiddleOfTheStreet,
    LivingRoomRightWindow,
    DownstairsHallWindow,
    GarageMainDoor
  }

  public class EnemyHandler : MonoBehaviour
  {
    [Space(5)]
    [Header("Animations References")]
    [SerializeField] private Animation animationComponent;
    [Space(2)]
    [SerializeField] private AnimationClip visionsOutsideDhallToEntranceClip;
    [SerializeField] private AnimationClip voicesMainGarageToEntranceClip;
    [SerializeField] private AnimationClip voicesGarageToDhallClip;
    [SerializeField] private AnimationClip voicesDhallToKichenClip;
    [SerializeField] private AnimationClip voicesKichenToKichenDepotClip;
    [SerializeField] private AnimationClip voicesKichenDepotToKichenTableClip;
    [SerializeField] private AnimationClip voicesKitchenTableToGarageClip;
    [Space(5)]
    [Header("Renderer References")]
    [SerializeField] private Renderer[] enemyRenderers;

    [Space(5)]
    [Header("Transforms")]
    [SerializeField] private Transform cornerTransform;
    [SerializeField] private Transform middleOfTheStreetTransform;
    [SerializeField] private Transform livingRoomRightWindowTransform;
    [SerializeField] private Transform downstairsHallWindowTransform;
    [SerializeField] private Transform garageMainDoorTransform;

    [Header("Menu audio")]
    [SerializeField] private AudioClip[] mosaicWalkingClip;
    [SerializeField, Range(0f, 1f)] private float mosaicWalkingVolume = 0.7f;
    [SerializeField] private AudioClip[] woodWalkingClip;
    [SerializeField, Range(0f, 1f)] private float woodWalkingVolume = 0.7f;

    private AudioSource enemySource;

    private ZoneSpotterHandler zoneSpotterHandler;

    private void Awake()
    {
      enemySource = gameObject.GetComponent<AudioSource>();
      zoneSpotterHandler = gameObject.GetComponent<ZoneSpotterHandler>();
    }

    private void Start()
    {
      HideEnemy();
    }

    private void OnDisable()
    {
      StopAllCoroutines();
    }

    public Transform GetCurrentEnemyTransform()
    {
      Transform headTransform = gameObject.transform.Find("Head");
      return headTransform;
    }

    public IEnumerator PlayEnemyAnimation(EnemyAnimations enemyAnimations, bool deactivateAfter)
    {
      AnimationClip clipToPlay;

      switch (enemyAnimations)
      {
        case EnemyAnimations.VisionsOutsideDhallToEntrance:
          clipToPlay = visionsOutsideDhallToEntranceClip;
          break;
        case EnemyAnimations.VoicesMainGarageToEntrance:
          clipToPlay = voicesMainGarageToEntranceClip;
          break;
        case EnemyAnimations.VoicesGarageToDhall:
          clipToPlay = voicesGarageToDhallClip;
          break;
        case EnemyAnimations.VoicesDhallToKichen:
          clipToPlay = voicesDhallToKichenClip;
          break;
        case EnemyAnimations.VoicesKichenToKichenDepot:
          clipToPlay = voicesKichenToKichenDepotClip;
          break;
        case EnemyAnimations.VoicesKichenDepotToKichenTable:
          clipToPlay = voicesKichenDepotToKichenTableClip;
          break;
        case EnemyAnimations.VoicesKitchenTableToGarage:
          clipToPlay = voicesKitchenTableToGarageClip;
          break;
        default:
          Debug.LogWarning("[EnemyHandler] Animación de enemigo inválida especificada.");
          yield break;
      }

      if (animationComponent != null)
      {
        animationComponent.Play(clipToPlay.name);

        yield return new WaitForSeconds(animationComponent[clipToPlay.name].length);

        if (deactivateAfter)
        {
          HideEnemy();
        }
      }
      else
      {
        yield return null;
      }
    }

    public IEnumerator PlayEnemyWalkingRoutine(FSMaterial fSMaterial)
    {
      AudioClip[] selectedClips;
      float targetVolume;

      switch (fSMaterial)
      {
        case FSMaterial.Wood:
          selectedClips = woodWalkingClip;
          targetVolume = woodWalkingVolume;
          break;
        case FSMaterial.MosaicGarage:
          selectedClips = mosaicWalkingClip;
          targetVolume = mosaicWalkingVolume;
          break;
        default:
          yield break;
      }
      while (true)
      {
        int randomIndex = Random.Range(0, selectedClips.Length);
        enemySource.clip = selectedClips[randomIndex];
        enemySource.volume = targetVolume;
        enemySource.Play();
        yield return new WaitForSeconds(enemySource.clip.length);

        // Intervalo entre pasos
        yield return new WaitForSeconds(0.5f);
      }
    }

    public void ShowEnemy(EnemyPositions position)
    {
      switch (position)
      {
        case EnemyPositions.PlayerHouseCorner:
          gameObject.transform.position = cornerTransform.position;
          gameObject.transform.rotation = cornerTransform.rotation;
          break;
        case EnemyPositions.MiddleOfTheStreet:
          gameObject.transform.position = middleOfTheStreetTransform.position;
          gameObject.transform.rotation = middleOfTheStreetTransform.rotation;
          break;
        case EnemyPositions.LivingRoomRightWindow:
          gameObject.transform.position = livingRoomRightWindowTransform.position;
          gameObject.transform.rotation = livingRoomRightWindowTransform.rotation;
          break;
        case EnemyPositions.DownstairsHallWindow:
          gameObject.transform.position = downstairsHallWindowTransform.position;
          gameObject.transform.rotation = downstairsHallWindowTransform.rotation;
          break;
        case EnemyPositions.GarageMainDoor:
          gameObject.transform.position = garageMainDoorTransform.position;
          gameObject.transform.rotation = garageMainDoorTransform.rotation;
          break;
        case EnemyPositions.None:
          HideEnemy();
          return;
        default:
          Debug.LogWarning("Posición de enemigo inválida especificada.");
          return;
      }

      // Mostramos al enemigo
      foreach (var renderer in enemyRenderers)
      {
        renderer.enabled = true;
      }

      if (zoneSpotterHandler != null) zoneSpotterHandler.canBeSpotted = true;
    }

    public void HideEnemy()
    {
      foreach (var renderer in enemyRenderers)
      {
        renderer.enabled = false;
      }
      zoneSpotterHandler.canBeSpotted = false;
    }

    // Calcula la duración exacta de la secuencia de invasión basada en los clips asignados
    public float GetInvasionSequenceDuration(float totalExtraTimes)
    {
      float totalDuration = 0f;

      totalDuration += totalExtraTimes;

      if (voicesMainGarageToEntranceClip != null)
      {
        totalDuration += voicesMainGarageToEntranceClip.length;
      }

      if (voicesGarageToDhallClip != null)
      {
        totalDuration += voicesGarageToDhallClip.length;
      }

      return totalDuration;
    }
  }
}