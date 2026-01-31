using System.Collections;
using TwelveG.AudioController;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.GameController
{
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
    [Space(10)]
    [Header("Enemy References")]
    [Space(5)]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] private Animation animationComponent;

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

    private void Awake()
    {
      enemySource = gameObject.GetComponent<AudioSource>();
    }

    public Transform GetCurrentEnemyTransform()
    {
      Transform headTransform = enemyPrefab.transform.Find("Head");
      return headTransform;
    }

    public IEnumerator PlayEnemyAnimation(string animationName, bool deactivateAfter)
    {
      if (animationComponent != null)
      {
        animationComponent.Play(animationName);

        if (deactivateAfter)
        {
          yield return new WaitForSeconds(animationComponent[animationName].length);
          enemyPrefab.SetActive(false);
        }
      }
      else
      {
        Debug.LogWarning("[EnvironmentHandler] Animation Component es nulo.");
      }

      if (!deactivateAfter) yield return null;
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
          enemyPrefab.transform.position = cornerTransform.position;
          enemyPrefab.transform.rotation = cornerTransform.rotation;
          break;
        case EnemyPositions.MiddleOfTheStreet:
          enemyPrefab.transform.position = middleOfTheStreetTransform.position;
          enemyPrefab.transform.rotation = middleOfTheStreetTransform.rotation;
          break;
        case EnemyPositions.LivingRoomRightWindow:
          enemyPrefab.transform.position = livingRoomRightWindowTransform.position;
          enemyPrefab.transform.rotation = livingRoomRightWindowTransform.rotation;
          break;
        case EnemyPositions.DownstairsHallWindow:
          enemyPrefab.transform.position = downstairsHallWindowTransform.position;
          enemyPrefab.transform.rotation = downstairsHallWindowTransform.rotation;
          break;
        case EnemyPositions.GarageMainDoor:
          enemyPrefab.transform.position = garageMainDoorTransform.position;
          enemyPrefab.transform.rotation = garageMainDoorTransform.rotation;
          break;
        case EnemyPositions.None:
          enemyPrefab.SetActive(false);
          return;
        default:
          Debug.LogWarning("Posición de enemigo inválida especificada.");
          return;
      }

      enemyPrefab.SetActive(true);

      var spotter = enemyPrefab.GetComponent<ZoneSpotterHandler>();
      if (spotter != null) spotter.canBeSpotted = true;
    }
  }
}