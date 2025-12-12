using UnityEngine;

namespace TwelveG.GameController
{
    // Acceso global estático a los eventos comunes del juego.
    // Cualquier script puede llamar a GameEvents.Common.NombreDelEvento
    public static class GameEvents
    {
        private static GlobalEventsRegistrySO _registry;

        public static GlobalEventsRegistrySO Common
        {
            get
            {
                // Lazy Loading
                if (_registry == null)
                {
                    _registry = Resources.Load<GlobalEventsRegistrySO>("GlobalEvents");
                    
                    if (_registry == null)
                        Debug.LogError("CRITICAL: No se encontró el archivo 'GlobalEvents' en la carpeta Resources.");
                }
                return _registry;
            }
        }
    }
}