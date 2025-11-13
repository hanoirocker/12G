using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class WalkieTalkie : PlayerItemBase
    {
        // [Header("WalkieTalkie Settings")]
        void Update()
        {
            if (itemIsShown)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    SwitchChannel(-1);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SwitchChannel(+1);
                }
            }
            else
            {
                // Lógica del walkie-talkie cuando está oculto o no funciona
            }
        }
        
        // Lógica para cambiar de canal en el walkie-talkie
        private void SwitchChannel(int direction)
        {
            // Chequea ultima posicion del canal escuchado

            // si el canal no es el ultimo o el primero, cambia al siguiente o anterior canal
            // si el canal posee un audio especial, reproduce el audio especial primero y luego el audio normal del canal
            // que queda en loop. Se debe guardar el instante de tiempo en el que se cambia de canal para continuar el audio especial o normal
            Debug.Log("Canal cambiado en dirección: " + direction);
        }
    }
}