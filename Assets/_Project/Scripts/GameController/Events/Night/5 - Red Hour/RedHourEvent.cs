using System;
using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

/**
 * NOTAS:
 * - Si el jugador va al garage, se reproduce un sonido de golpes en la puerta del garage.
 * - Independientemente de lo anterior, se abre la puerta del cuarto de los padres y empieza a parpadear la luz.
 */
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
    [SerializeField] private AudioClip garageDoorKnockingClip;
    [SerializeField, Range(0f, 1f)] private float garageDoorKnockingVolume = 0.15f;

    private bool allowNextAction = false;

    public override IEnumerator Execute()
    {
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
      // Se habilitan colliders de garage para que luego se ejecute "OnGarageEntered".
      // Estos colliders se desactivan luego de que el jugador entre al garage.
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Garage Colliders", true));

      yield return new WaitForSeconds(initialTime);

      // Rutina de puerta de habitación de los padres
      Coroutine mainCoroutine = StartCoroutine(ParentsBedRoomRoutine());

      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);
      yield return new WaitUntil(() => PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.UpstairsHall);
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

      // Espera a que haya terminado la rutina principal
      yield return mainCoroutine;

      // Recibe "OnParentsNightmareCollidersTriggered" para continuar la secuencia.
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
    }

    private IEnumerator ParentsBedRoomRoutine()
    {
      GameObject parentsLight = PlayerHouseHandler.Instance.GetStoredObjectByID("Parents light");
      float originalLightIntensity = parentsLight.GetComponent<Light>().intensity;
      float originalLightColorTemperature = parentsLight.GetComponent<Light>().colorTemperature;
      float originalLightMaximumRange = parentsLight.GetComponent<Light>().range;

      // Hace empezar a parpadear la luz del cuarto de los padres
      parentsLight.GetComponent<Light>().intensity = originalLightIntensity * 2f;
      parentsLight.GetComponent<Light>().range = originalLightMaximumRange * 2;
      parentsLight.GetComponent<LightFlickeringHandler>().StartFlickering();

      // Destraba y abre la puerta del cuarto de los padres
      PlayerHouseHandler.Instance.GetStoredObjectByID("Parents Door Lock").GetComponent<DownstairsOfficeDoorHandler>().UnlockDoorByEvent();
      yield return new WaitForSeconds(3f);
      PlayerHouseHandler.Instance.GetStoredObjectByID("Parents Door Lock").GetComponent<DownstairsOfficeDoorHandler>().Interact(null);

      // Espera a que el jugador entre al cuarto de los padres
      yield return new WaitUntil(() => PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.ParentsBedroom);

    }

    // Recibe "OnGarageCollidersTriggered" para disparar sonido enemigo golpeado
    // puerta del garage.
    public void OnGarageEntered()
    {
      Transform garageGateTransform = PlayerHouseHandler.Instance.GetTransformByObject(HouseObjects.GarageGate);
      StartCoroutine(HandleEventSoundsRoutine(garageGateTransform, garageDoorKnockingClip, garageDoorKnockingVolume));
      garageColliderListener.enabled = false;
    }

    private IEnumerator HandleEventSoundsRoutine(Transform transform, AudioClip clip, float volume)
    {

      if (transform)
      {

        (AudioSource audioSource, AudioSourceState audioState) =
          AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
            transform, volume
        );

        if (audioSource != null && clip != null)
        {
          audioSource.clip = clip;
          audioSource.loop = false;

          yield return new WaitForSeconds(3f); // Esperar un poco antes de reproducir el sonido
          audioSource.Play();
        }

        yield return new WaitForSeconds(clip.length);
        AudioUtils.StopAndRestoreAudioSource(audioSource, audioState);
      }
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