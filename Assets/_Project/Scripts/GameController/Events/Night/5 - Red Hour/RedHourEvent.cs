using System;
using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.GameController
{
  public class RedHourEvent : GameEventBase
  {
    [Header("References")]
    [SerializeField] private GameEventListener garageColliderListener;
    [Space(10)]
    [Header("Event options")]
    [SerializeField, Range(1, 10)] private int initialTime = 0;

    [Space(10)]
    [Header("Text event SO")]
    [SerializeField] private UIOptionsTextSO[] playerHelperDataTextSO;
    [Space(5)]
    [SerializeField] private ObservationTextSO[] observationTextSOs;
    [Space(5)]
    [SerializeField] private DialogSO[] dialogSOs;
    [SerializeField] private DialogSO dialogFromDownstairsSO;

    [Space(10)]
    [Header("Audio Options")]
    [SerializeField] private AudioClip garageDoorKnockingClip; // TODO: Reemplazar por el clip adecuado
    [SerializeField, Range(0f, 1f)] private float garageDoorKnockingVolume = 0.15f;

    private bool allowNextAction = false;

    public override IEnumerator Execute()
    {
      PlayerHouseHandler playerHouseHandler = PlayerHouseHandler.Instance;
      // Estos colliders se desactivan luego de que el jugador entre al garage
      playerHouseHandler.ToggleStoredPrefabs(new ObjectData("Garage Colliders", true));

      yield return new WaitForSeconds(initialTime);

      // BLA
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
    }

    // Recibe "OnGarageCollidersTriggered" para disparar sonido enemigo golpeado
    // puerta del garage
    public void OnGarageEntered()
    {
      StartCoroutine(HandleGarageDoorKnockingSound());
    }

    private IEnumerator HandleGarageDoorKnockingSound()
    {
      Debug.Log("RedHourEvent: OnGarageCollidersTriggered.");
  
      Transform garageDoorTransform = PlayerHouseHandler.Instance.GetTransformByObject(HouseObjects.GarageDoor);

      if (garageDoorTransform)
      {

        (AudioSource garageSource, AudioSourceState garageState) =
          AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
            garageDoorTransform, garageDoorKnockingVolume
        );

        if (garageSource != null && garageDoorKnockingClip != null)
        {
          garageSource.clip = garageDoorKnockingClip;
          garageSource.loop = false;

          yield return new WaitForSeconds(3f); // Esperar un poco antes de reproducir el sonido
          garageSource.Play();
        }

        yield return new WaitForSeconds(garageDoorKnockingClip.length);
        AudioUtils.StopAndRestoreAudioSource(garageSource, garageState);
      }

      garageColliderListener.enabled = false;

      yield break;
    }

    public void AllowNextActions(Component sender, object data)
    {
      allowNextAction = true;
    }

    public void ResetAllowNextActions()
    {
      allowNextAction = false;
    }
  }
}