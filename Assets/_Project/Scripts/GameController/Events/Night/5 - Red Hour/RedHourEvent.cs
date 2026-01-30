using System;
using System.Collections;
using Cinemachine;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using TwelveG.VFXController;
using UnityEngine;

namespace TwelveG.GameController
{
  public class RedHourEvent : GameEventBase
  {
    [Header("References")]
    [SerializeField] private GameEventListener garageColliderListener;

    [Space(10)]
    [Header("Event options")]
    [SerializeField, Range(1f, 15f)] private float initialTime = 5f;
    [SerializeField, Range(0f, 10f)] private float timeUntilSecondDistortion = 0.5f;

    [Space(10)]
    [Header("Text event SO")]
    [SerializeField] private UIOptionsTextSO[] playerHelperDataTextSO;
    [Space(5)]
    [SerializeField] private ObservationTextSO[] observationTextSOs;
    [Space(5)]
    [SerializeField] private DialogSO[] dialogSOs;

    [Space(10)]
    [Header("Audio Options")]
    [SerializeField] private AudioClip garageDoorKnockingClip;
    [SerializeField, Range(0f, 1f)] private float garageDoorKnockingVolume = 0.15f;

    private bool allowNextAction = false;

    public override IEnumerator Execute()
    {
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

      AlternatePortraits(false);
      // Se habilitan colliders de garage para que luego se ejecute "OnGarageEntered".
      // Estos colliders se desactivan luego de que el jugador entre al garage.
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Garage Colliders", true));

      yield return new WaitForSeconds(initialTime);

      // Destraba y abre la puerta del cuarto de los padres
      yield return PlayerHouseHandler.Instance.GetStoredObjectByID("Parents Door Lock").GetComponent<DownstairsOfficeDoorHandler>().UnlockDoorByEventCoroutine();
      yield return new WaitForSeconds(3f);
      PlayerHouseHandler.Instance.GetStoredObjectByID("Parents Door Lock").GetComponent<DownstairsOfficeDoorHandler>().Interact(null);
      yield return new WaitForSeconds(3f);

      // Empiezan a brillar cuadros y se descontrola el WT
      GetComponent<GlowingPortraitsHandler>().StartGlowingRoutine();
      PlayerHandler.Instance.GetComponentInChildren<WalkieTalkie>().StartRandomChannelSwitching();

      Coroutine mainCoroutine = StartCoroutine(ParentsBedRoomRoutine());

      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

      yield return new WaitUntil(() => PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.UpstairsHall);
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

      // Espera a que haya terminado la rutina principal
      // y luego detiene la rutina de mal funcionamiento del WT
      yield return mainCoroutine;
      PlayerHandler.Instance.GetComponentInChildren<WalkieTalkie>().StopRandomChannelSwitching();

      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[2]);

      GetComponent<GlowingPortraitsHandler>().StopGlowingRoutine();

      // Qué carajo fue eso
      GameEvents.Common.onStartDialog.Raise(this, dialogSOs[0]);

      // "conversationHasEnded"
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
    }

    private void AlternatePortraits(bool toNormalPortraits)
    {
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Old Woman Portrait", toNormalPortraits));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Rose Portrait", toNormalPortraits));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Birds Portrait", toNormalPortraits));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Flowers Portrait", toNormalPortraits));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Woods Portrait", toNormalPortraits));

      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Tonges Portrait", !toNormalPortraits));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Scattered Lady Portrait", !toNormalPortraits));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Scared Lady Portrait", !toNormalPortraits));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Snake Portrait", !toNormalPortraits));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Heart Portrait", !toNormalPortraits));
    }

    private IEnumerator ParentsBedRoomRoutine()
    {
      GameObject parentsLight = PlayerHouseHandler.Instance.GetStoredObjectByID("Parents light");
      float originalParentsLightIntensity = parentsLight.GetComponent<Light>().intensity;
      float originalParentsLightColorTemperature = parentsLight.GetComponent<Light>().colorTemperature;
      float originalParentsLightMaximumRange = parentsLight.GetComponent<Light>().range;
      // Hace empezar a parpadear la luz del cuarto de los padres
      parentsLight.GetComponent<Light>().intensity = originalParentsLightIntensity * 3f;
      parentsLight.GetComponent<Light>().range = originalParentsLightMaximumRange * 4;
      parentsLight.GetComponent<LightFlickeringHandler>().StartFlickering();

      GameObject zoomLight = PlayerHouseHandler.Instance.GetStoredObjectByID("Zoom light");
      float originalZoomLightIntensity = zoomLight.GetComponent<Light>().intensity;
      float originalZoomLightColorTemperature = zoomLight.GetComponent<Light>().colorTemperature;

      zoomLight.GetComponent<Light>().intensity = originalZoomLightIntensity * 5f;
      zoomLight.GetComponent<Light>().color = Color.red;
      zoomLight.GetComponent<LightFlickeringHandler>().StartFlickering();

      // Activa el collider spotteable de la pieza de los padres y el collider de ubicación del jugador
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Red Hour - Spottable", true));
      ISpot spotteablePortrait = PlayerHouseHandler.Instance.GetStoredObjectByID("Red Hour - Spottable").GetComponent<ISpot>();
      ISpot spotteableStatue = PlayerHouseHandler.Instance.GetStoredObjectByID("Sculpture").GetComponent<ISpot>();
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Red Hour - Collider", true));
      EventTriggeredByColliders zoneTrigger = PlayerHouseHandler.Instance.GetStoredObjectByID("Red Hour - Collider").GetComponent<EventTriggeredByColliders>();

      // Activa objeto de pajaros muertos en el Zoom
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Zoom - Dead Birds", true));

      // Espera para apagar la luz y alternar entre los muebles ordenados y desordenados
      yield return new WaitUntil(() =>
      {
        bool playerInZoom = PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.Zoom;
        bool isLookingAtTarget = PlayerHandler.Instance.PlayerSpotter.GetCurrentlySpottedObject() == spotteableStatue;

        return playerInZoom && isLookingAtTarget;
      });

      parentsLight.GetComponent<LightFlickeringHandler>().StopFlickering(false);
      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]);

      // Cambiamos el cuadro normal por el cuadro con la cara rara
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Mother Portrait", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Empty Face Portrait", true));

      // Espera a que el jugador spottee el cuadro dentro del collider frente a la cama
      yield return new WaitUntil(() =>
      {
        bool inZone = zoneTrigger.IsPlayerInside;
        bool isLookingAtTarget = PlayerHandler.Instance.PlayerSpotter.GetCurrentlySpottedObject() == spotteablePortrait;

        return inZone && isLookingAtTarget;
      });

      // Cancelar contemplaciones si se estaban mostrando
      if(UIManager.Instance.ContemplationCanvasHandler.IsShowingText())
      {
        UIManager.Instance.ContemplationCanvasHandler.HideContemplationCanvas();
      }

      // Indica a la Virtual Camera activa que debe mirar hacia el cuadro
      GameEvents.Common.onVirtualCamerasControl.Raise(
        this, new LookAtTarget(
          PlayerHouseHandler.Instance.GetStoredObjectByID(
            "Empty Face Portrait"
          ).transform
        )
      );
      GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseIn, 1));

      // Vuelve a los cuadros originales y detiene el parpadeo de las luces
      AlternatePortraits(true);
      zoomLight.GetComponent<LightFlickeringHandler>().StopFlickering(false);

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
      VirtualCamerasHandler.Instance.ToggleActiveCameraNoise(false);

      parentsLight.GetComponent<Light>().color = Color.red;
      parentsLight.GetComponent<Light>().intensity = 55f;
      parentsLight.GetComponent<Light>().enabled = true;
      yield return StartCoroutine(VFXManager.Instance.TriggerHallucinationEffect(HallucinationEffectType.NPRedDistortionFadeIn));
      parentsLight.GetComponent<Light>().enabled = false;
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Empty Face Portrait", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Mother Portrait", true));

      yield return new WaitForSeconds(timeUntilSecondDistortion);

      // Flash visual de Fernandez y espera
      parentsLight.GetComponent<Light>().enabled = true;
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Mother Portrait", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Organized Objects", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Messy Objects", true));
      yield return StartCoroutine(VFXManager.Instance.TriggerHallucinationEffect(HallucinationEffectType.FernandezHallucination));

      // Vuelve todo a la normalidad con un efecto de Red Distortion Fade Out
      parentsLight.GetComponent<Light>().enabled = false;
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Messy Objects", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Organized Objects", true));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Mother Portrait", true));
      StartCoroutine(VFXManager.Instance.TriggerHallucinationEffect(HallucinationEffectType.RedDistortionFadeOut));

      // Resetea la visual del jugador
      GameEvents.Common.onVirtualCamerasControl.Raise(this, new LookAtTarget(null));
      GameEvents.Common.onMainCameraSettings.Raise(this, new ResetCinemachineBrain());

      // Restaura la luz del cuarto de los padres y zoom a su estado original
      parentsLight.GetComponent<Light>().colorTemperature = originalParentsLightColorTemperature;
      parentsLight.GetComponent<Light>().intensity = originalParentsLightIntensity;
      parentsLight.GetComponent<Light>().range = originalParentsLightMaximumRange;
      zoomLight.GetComponent<Light>().intensity = originalZoomLightIntensity;
      zoomLight.GetComponent<Light>().colorTemperature = originalZoomLightColorTemperature;

      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Red Hour - Spottable", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Red Hour - Collider", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Zoom - Dead Birds", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Sculpture", false));

      VirtualCamerasHandler.Instance.ToggleActiveCameraNoise(true);
      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
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