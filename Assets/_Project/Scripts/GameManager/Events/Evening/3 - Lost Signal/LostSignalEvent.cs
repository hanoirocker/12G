namespace TwelveG.GameManager
{
    using System.Collections;
    using Cinemachine;
    using TwelveG.PlayerController;
    using TwelveG.UIManagement;
    using UnityEngine;

    public class LostSignalEvent : GameEventBase
    {
        //TODO: LOCALIZATION!
        [Header("Event options")]
        [SerializeField, Range(1,10)] private int initialTime = 1;

        [Header("EventsSO references")]
        public GameEventSO onObservationCanvasShowText;

        [Header("Other eventsSO references")]
        public GameEventSO enablePC;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ LOST SIGNAL 1 EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);
            onObservationCanvasShowText.Raise(this, "PRIMER DIALOGO DE LOSTSIGNAL1");

            // Habilita el interactuable de la pc y desactiva el contemplable
            enablePC.Raise(this, null);

            // Unity Event (PCHandler - onPC):
            // Al interactuar con PCHandler se deshabilitan controles pples 
            // del jugador y se pasa a la VC "PC VC"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Aca sucede PCHandler.

            // Unity Event (PCHandler - onPC):
            // El jugador abandona la PC y vuelve a retomar control
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

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