namespace TwelveG.GameController
{
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.Localization;
    using TwelveG.UIController;
    using Cinemachine;
    using TwelveG.PlayerController;
    using TwelveG.Utils;
    using UnityEngine;

    public class LostSignal2Event : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("Text event SO")]
        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;

        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO updateFallbackTexts;
        [SerializeField] private GameEventSO enableBackpack;
        [SerializeField] private GameEventSO disableBackpack;
        [SerializeField] private GameEventSO enablePhone;
        [SerializeField] private GameEventSO onVirtualCamerasControl;
        [SerializeField] private GameEventSO onMainCameraSettings;
        [SerializeField] private GameEventSO onImageCanvasControls;
        [SerializeField] private GameEventSO onPlayerControls;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ LOST SIGNAL 2 EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);

            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            // Esto sera recibido por Backpack para activar el interactuable
            // y desactivar el contemplable.
            enableBackpack.Raise(this, null);
            enablePhone.Raise(this, null);

            // ¡Mi teléfono! Necesito hablar con Mica y ver si soy el único con esta suerte. Pero .. ¿Dónde lo habré dejado?
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );

            yield return new WaitForSeconds(3f);

            // Unity Event (PhonePrefabHandler - finishedUsingPhone):
            // Se recibe cuando el jugador deja de usar el teléfono
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Retorna a la camara del player desde la del sofá
            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));
            yield return new WaitForSeconds(1f);
            onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.Cut, 0));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Sofa, false));
            yield return new WaitForSeconds(1f);
            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));

            // Nos aseguramos que el interactuable del Backpack se destruya
            // si no fue revisada hasta este punto.
            disableBackpack.Raise(this, null);

            // Espera un toque para que el jugador pueda leer el texto antes de activar mandos
            onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            yield return new WaitForSeconds(2f);

            // YA FUE, me voy a lo de Mica.
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[1]
            );
            yield return new WaitForSeconds(2f);
        }

        public void AllowNextActions(Component sender, object data)
        {
            print(gameObject.name + "recieved event sent by: " + sender.gameObject.name);
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }
    }
}