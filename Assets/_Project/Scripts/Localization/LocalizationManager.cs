namespace TwelveG.Localization
{
    using TwelveG.SaveSystem;
    using UnityEngine;
    using UnityEngine.Localization.Settings;

    public class LocalizationManager : MonoBehaviour, IDataPersistence
    {
        public static LocalizationManager Instance { get; private set; }

        [Header("Game Event SO's")]
        public GameEventSO onLanguageChanged;

        private string currentLanCode;

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

            currentLanCode = GetCurrentLanguageCode();
        }

        public void ChangeLanguage(string languageCode)
        {
            // print("Seteando lenguage a: " + languageCode);
            var locale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
                currentLanCode = languageCode;
                onLanguageChanged.Raise(this, null);
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

        public void LoadData(GameData gameData)
        {
            ChangeLanguage(gameData.languageCode);
        }

        public void SaveData(ref GameData gameData)
        {
            gameData.languageCode = currentLanCode;
        }
    }
}