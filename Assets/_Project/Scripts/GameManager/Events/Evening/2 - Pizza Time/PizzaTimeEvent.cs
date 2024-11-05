namespace TwelveG.GameManager
{
    using System.Collections;
    using UnityEngine;

    public class PizzaTimeEvent : GameEventBase
    {
        //TODO: LOCALIZATION! eventtextsSO
        [Header("Event options")]
        [TextArea(5, 50)]
        [SerializeField, Range(1, 10)] private int initialTime = 4;

        [Header("Objects options")]
        [SerializeField] private GameObject policeCar;

        [Header("EventsSO references")]
        public GameEventSO onObservationCanvasShowText;

        [Header("Other eventsSO references")]
        public GameEventSO plateCanBePicked;
        public GameEventSO pizzaCanBePicked;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ PIZZA TIME EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);
            onObservationCanvasShowText.Raise(this, "PRIMER DIALOGO DE PIZZATIME");

            // Esto notifica al plato para que haga AllowItemToBePicked = true;
            pizzaCanBePicked.Raise(this, null);

            plateCanBePicked.Raise(this, true);

            // Unity Event (MicrowaveHandler - pizzaHeatingFinished):
            // Se muestra el diálogo de felicidad del pelotudo
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(0.5f);
            onObservationCanvasShowText.Raise(this, "SEGUNDO DIALOGO DE PIZZATIME");

            // Unity Event (SitAndEat - sitAndEatPizza):
            // Al interactuar con SitAndEat (mesa de la cocina) se deshabilitan controles pples 
            // del jugador y se pasa a la VC KitchenDeskVC
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Aca sucede PizzaSliceHandler

            // Unity Event (PizzaSliceHandler - finishedEatingPizza):
            // Se invoca finishedEatingPizza al final de comer la pizza para que el jugador vuelva
            // a estar disponible.
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