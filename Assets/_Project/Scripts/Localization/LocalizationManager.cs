namespace TwelveG.Localization
{
    using UnityEngine;
    using UnityEngine.Localization.Settings;

    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        public GameEventSO onLanguageChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ChangeLanguage(string languageCode)
        {
            var locale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
                onLanguageChanged.Raise(this, GetCurrentLanguageCode());
            }
        }

        public string GetCurrentLanguageCode()
        {
            return LocalizationSettings.SelectedLocale.Identifier.Code;
        }

        public void SetLanguageToEnglish()
        {
            ChangeLanguage("en");
        }

        public void SetLanguageToSpanish()
        {
            ChangeLanguage("es");
        }
    }
}