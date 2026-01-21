using System;
using System.Collections;
using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.Utils;
using TwelveG.VFXController;
using UnityEngine;

namespace TwelveG.GameController
{
  [Serializable]
  public struct PortraitGlowRule
  {
    public HouseArea[] validAreas;
    public string[] portraitIDs;
  }

  public class RedHourEvent : GameEventBase
  {
    [Header("References")]
    [SerializeField] private GameEventListener garageColliderListener;

    [Space(10)]
    [Header("Glow Logic Configuration")]
    [SerializeField] private List<PortraitGlowRule> portraitGlowRules;

    [Space(10)]
    [Header("Event options")]
    [SerializeField, Range(1f, 15f)] private float initialTime = 5f;

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
    private bool stopGlowingRoutine = false;

    public override IEnumerator Execute()
    {
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
      AlternatePortraits();
      // Se habilitan colliders de garage para que luego se ejecute "OnGarageEntered".
      // Estos colliders se desactivan luego de que el jugador entre al garage.
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Garage Colliders", true));

      yield return new WaitForSeconds(initialTime);

      stopGlowingRoutine = false;

      Coroutine portraitsGlowingCoroutine = StartCoroutine(HandlePortraitsGlowingRoutine());

      Coroutine mainCoroutine = StartCoroutine(ParentsBedRoomRoutine());

      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

      yield return new WaitUntil(() => PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.UpstairsHall);
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

      // Espera a que haya terminado la rutina principal
      yield return mainCoroutine;

      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[2]);

      // DETENEMOS LA RUTINA DE GLOW ACA
      // Una vez que termina la rutina principal (ParentsBedRoomRoutine), detenemos la rutina de glow
      stopGlowingRoutine = true;
      StopCoroutine(portraitsGlowingCoroutine);

      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
    }

    private void AlternatePortraits()
    {
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Old Woman Portrait", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Rose Portrait", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Birds Portrait", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Flowers Portrait", false));

      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Tonges Portrait", true));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Scattered Lady Portrait", true));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Scared Lady Portrait", true));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Snake Portrait", true));
    }

    private IEnumerator ParentsBedRoomRoutine()
    {
      // Destraba y abre la puerta del cuarto de los padres
      PlayerHouseHandler.Instance.GetStoredObjectByID("Parents Door Lock").GetComponent<DownstairsOfficeDoorHandler>().UnlockDoorByEvent();
      yield return new WaitForSeconds(3f);
      PlayerHouseHandler.Instance.GetStoredObjectByID("Parents Door Lock").GetComponent<DownstairsOfficeDoorHandler>().Interact(null);
      yield return new WaitForSeconds(0.5f);

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

      zoomLight.GetComponent<Light>().intensity = originalZoomLightIntensity * 3.5f;
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

      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]);

      parentsLight.GetComponent<LightFlickeringHandler>().StopFlickering(false);

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

      zoomLight.GetComponent<LightFlickeringHandler>().StopFlickering(false);

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
      VirtualCamerasHandler.Instance.ToggleActiveCameraNoise(false);

      parentsLight.GetComponent<Light>().color = Color.red;
      parentsLight.GetComponent<Light>().intensity = 55f;
      parentsLight.GetComponent<Light>().enabled = true;
      VFXManager.Instance.SetRedHourIntensity(1f, 1f);
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Organized Objects", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Messy Objects", true));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Empty Face Portrait", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Bigger Empty Face Portrait", true));
      yield return new WaitForSeconds(1f);
      parentsLight.GetComponent<Light>().enabled = false;
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Messy Objects", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Organized Objects", true));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Bigger Empty Face Portrait", false));
      yield return new WaitForSeconds(1f);
      Debug.Log("ENEMIGO EN FRENTE");
      parentsLight.GetComponent<Light>().enabled = true;
      VFXManager.Instance.SetRedHourIntensity(1f, 0f);
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Messy Objects", true));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Bigger Empty Face Portrait", true));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Organized Objects", false));
      yield return new WaitForSeconds(1f);
      parentsLight.GetComponent<Light>().enabled = false;
      VFXManager.Instance.SetRedHourIntensity(0f, 10f);
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Messy Objects", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Parents - Organized Objects", true));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Bigger Empty Face Portrait", false));
      PlayerHouseHandler.Instance.ToggleStoredPrefabs(new ObjectData("Mother Portrait", true));
      Debug.Log("Orden en la pieza");

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

    private IEnumerator HandlePortraitsGlowingRoutine()
    {
      HouseArea lastArea = HouseArea.None;
      List<string> currentActivePortraits = new List<string>();

      while (!stopGlowingRoutine)
      {
        HouseArea currentArea = PlayerHandler.Instance.GetCurrentHouseArea();

        if (currentArea != lastArea)
        {
          foreach (string id in currentActivePortraits)
          {
            SetPortraitGlow(id, false);
          }
          currentActivePortraits.Clear();

          foreach (var rule in portraitGlowRules)
          {
            bool areaMatches = Array.Exists(rule.validAreas, area => area == currentArea);

            if (areaMatches)
            {
              foreach (string id in rule.portraitIDs)
              {
                SetPortraitGlow(id, true);
                currentActivePortraits.Add(id);
              }
              break;
            }
          }

          lastArea = currentArea;
        }

        yield return new WaitForSeconds(0.2f);
      }

      foreach (string id in currentActivePortraits) SetPortraitGlow(id, false);
    }

    private void SetPortraitGlow(string objectID, bool enable)
    {
      GameObject portrait = PlayerHouseHandler.Instance.GetStoredObjectByID(objectID);
      PulsingGlowHandler glowHandler = portrait.GetComponentInChildren<PulsingGlowHandler>();

      if (portrait != null)
      {
        if (glowHandler != null)
        {
          if (enable)
          {
            glowHandler.enabled = true;
          }
          else
          {
            glowHandler.TurnOffGlow();
          }
        }
      }
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