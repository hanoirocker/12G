namespace TwelveG.Localization
{
    using UnityEngine;

    public class LocalizationData : MonoBehaviour
    {
        private string currentLanguage;

        public string CurrentLanguage
        {
            get { return currentLanguage; }
            private set { currentLanguage = value; }
        }

        public void SetCurrentLanguage(Component sender, object data)
        {
            CurrentLanguage = (string)data;
        }
    }
}