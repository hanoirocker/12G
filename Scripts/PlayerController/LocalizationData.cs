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
            // print("Recibiendo en " + gameObject.name + " Localization Data script, desde " + sender.gameObject.name + ", lang code: " + (string)data);
            CurrentLanguage = (string)data;
        }
    }
}