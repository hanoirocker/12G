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

    private void Start()
    {
        onLanguageChanged.Raise(this, GetCurrentLanguageCode());
        print("Enviando desde LocalizationManager, lang code: " + GetCurrentLanguageCode());
    }


    public void ChangeLanguage(string languageCode)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            onLanguageChanged.Raise(this, GetCurrentLanguageCode());
        }

        print("Language now is: " + GetCurrentLanguageCode());
    }

    public string GetCurrentLanguageCode()
    {
         // Retorna el código del idioma activo
        return LocalizationSettings.SelectedLocale.Identifier.Code;
    }

    // Función para cambiar de idioma en botones del Language Canvas
    public void SetLanguageToEnglish()
    {
        ChangeLanguage("en");
    }

    // Función para cambiar de idioma en botones del Language Canvas
    public void SetLanguageToSpanish()
    {
        ChangeLanguage("es");
    }
}
