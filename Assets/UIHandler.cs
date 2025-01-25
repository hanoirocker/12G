namespace TweleveG.UIManagement
{
    using System.Collections.Generic;
    using UnityEngine;

    public class UIHandler : MonoBehaviour
    {
         [SerializeField] private GameEventSO updateCanvasTextOnLanguageChanged;

        public void UpdateCanvasOnLanguageChanged(Component sender, object data)
        {
            updateCanvasTextOnLanguageChanged.Raise(this, data);
        }
    }
}