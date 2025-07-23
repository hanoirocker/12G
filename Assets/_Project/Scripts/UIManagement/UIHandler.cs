namespace TwelveG.UIController
{
    using TwelveG.Localization;
    using UnityEngine;

    public class UIHandler : MonoBehaviour
    {
        private string currentLanguage;

        public string CurrentLanguage
        {
            get { return currentLanguage; }
            private set { currentLanguage = value; }
        }

        private void Start()
        {
            CurrentLanguage = LocalizationManager.Instance.GetCurrentLanguageCode();
        }

        public void UpdateCurrentUILanguage(Component sender, object data)
        {
            string newLanguageSet = (string)data;

            if (CurrentLanguage == newLanguageSet) { return; }

            CurrentLanguage = newLanguageSet;
        }
    }
}