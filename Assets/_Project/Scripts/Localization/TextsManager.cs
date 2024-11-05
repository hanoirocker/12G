namespace TwelveG.Localization
{
    using TwelveG.InteractableObjects;
    using UnityEngine;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Settings;

    public class TextsManager : MonoBehaviour
    {
        [SerializeField] private LocalizedString interactableText;
        [SerializeField] private LocalizedString eventText;

        private LocalizationManager localizationManager;

        private void Awake()
        {
            localizationManager = GetComponent<LocalizationManager>();
        }

        public string ReturnInteractionText(Component sender, object data)
        {
            if (data is InteractionTextsSO interactionTextsSO)
            {
                string entryKey = interactionTextsSO.objectKey;
                string languageCode = localizationManager.GetCurrentLanguageCode();

                return GetTextFromInteractableTable(entryKey, languageCode);
            }
            else
            {
                Debug.LogWarning("El objeto interactuable no envió un elemento InteractionTextsSO");
                return null;
            }
        }

        private string GetTextFromInteractableTable(string entryKey, string languageCode)
        {
            string tableName = interactableText.TableReference.TableCollectionName;

            print("Valor encontrado: " + LocalizationSettings.StringDatabase.GetLocalizedString(tableName, entryKey));

            return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, entryKey);

        }
    }
}
