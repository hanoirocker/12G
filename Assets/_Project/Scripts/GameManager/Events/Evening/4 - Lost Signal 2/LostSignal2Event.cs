namespace TwelveG.GameManager
{
    using System.Collections;
    using UnityEngine;

    public class LostSignal2Event : GameEventBase
    {
        [Header("References")]
        [SerializeField] private GameObject carAlarmCollidersPrefab;

        [Header("Event options")]
        [TextArea(2, 25)]
        [SerializeField] private string defaultEventControlOptions;
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("Other eventsSO references")]
        public GameEventSO onObservationCanvasShowText;
        public GameEventSO enableBackpack;
        public GameEventSO disableBackpack;
        public GameEventSO enablePhone;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ LOST SIGNAL 2 EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);
            // Esto sera recibido por Backpack para activar el interactuable
            // y desactivar el contemplable.
            enableBackpack.Raise(this, null);
            enablePhone.Raise(this, null);

            // DEBE ESTAR POR ALGUN LADO?
            onObservationCanvasShowText.Raise(this, "BUSCAR TELEFONO: LOCALIZATION");

            yield return new WaitForSeconds(3f);

            // Unity Event (PhonePrefabHandler - finishedUsingPhone):
            // Se recibe cuando el jugador deja de usar el teléfono
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Nos aseguramos que el interactuable del Backpack se destruya
            // si no fue revisada hasta este punto.
            disableBackpack.Raise(this, null);

            // YA FUE, VOY A LO DE MICA.
            onObservationCanvasShowText.Raise(this, "VOY A LO DE MICA: LOCALIZATION");

            // Aca deberia pasar el sucidio de Fernandez.
            Instantiate(carAlarmCollidersPrefab);

            // Unity Event (AlarmHandler - carAlarmStopped):
            // Se recibe cuando la alarma deja de sonar
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