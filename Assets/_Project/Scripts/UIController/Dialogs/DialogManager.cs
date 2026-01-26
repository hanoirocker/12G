using System.Collections;
using System.Collections.Generic;
using TMPro;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.DialogsController
{
    public struct DialogForChannel
    {
        public int channelIndex;
        public DialogSO dialogSO;
    }

    public class DialogManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private GameObject buttonPrefab;

        [SerializeField] private Canvas dialogCanvas;
        [SerializeField] private TextMeshProUGUI dialogCanvasText;
        [SerializeField] private TextMeshProUGUI characterNameCanvasText;

        private DialogSO currentDialog;
        private List<DialogOptions> currentOptions;

        private void OnEnable()
        {
            UpdateCanvasTextOnLanguageChanged();
        }

        void Start()
        {
            dialogCanvas.enabled = false;
        }

        public void HideDialog()
        {
            dialogCanvas.enabled = false;
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            if (currentDialog == null) return;

            string textToShow = Utils.TextFunctions.RetrieveDialogText(
                LocalizationManager.Instance.GetCurrentLanguageCode(),
                currentDialog
            );

            dialogCanvasText.text = textToShow;
        }

        public bool DialogIsShowing()
        {
            return dialogCanvasText.gameObject.activeSelf;
        }

        public IEnumerator ShowDialogCoroutine(string charName, string dialog, float audioClipDuration)
        {
            characterNameCanvasText.text = charName + ":";
            dialogCanvasText.text = dialog;

            dialogCanvas.enabled = true;
            dialogPanel.SetActive(true);

            yield return new WaitForSeconds(audioClipDuration);

            dialogPanel.SetActive(false);
        }

        private IEnumerator StartDialogCoroutine(DialogSO currentDialog)
        {
            float timeBeforeShowing = currentDialog.timeBeforeShowing;
            currentOptions = currentDialog.dialogOptions;

            yield return new WaitForSeconds(timeBeforeShowing);

            // Si NO es un pensamiento interno, asumimos que es audio que debe pausar la estática.
            // Esto incluye a Mica, Simon (hablando), Policia, Unknown, etc.
            bool shouldPauseStatic = !currentDialog.isSelfDialog;

            if (shouldPauseStatic)
            {
                // Enviamos TRUE a WT: El diálogo está corriendo, silencia la estática de fondo
                GameEvents.Common.onDialogNodeRunning.Raise(this, true);
            }

            // --- Preparación de Texto y Audio ---
            string textToShow = Utils.TextFunctions.RetrieveDialogText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    currentDialog
                );

            // Beep inicial de Simon
            if (currentDialog.characterName == CharacterName.Simon && !currentDialog.isSelfDialog)
            {
                yield return StartCoroutine(AudioManager.Instance.AudioDialogsHandler.PlayBeepSound());
            }

            float dialogTime = currentDialog.spanishDialogClip != null ? currentDialog.spanishDialogClip.length : Utils.TextFunctions.CalculateTextDisplayDuration(textToShow);

            // Reproducción del clip de voz
            if (currentDialog.spanishDialogClip != null)
            {
                bool isSimon = currentDialog.characterName == CharacterName.Simon;
                StartCoroutine(AudioManager.Instance.AudioDialogsHandler.PlayDialogClip(currentDialog.spanishDialogClip, isSimon: isSimon));
            }

            // Evento de inicio (ej: animaciones)
            if (currentDialog.startingEvent != null)
            {
                currentDialog.startingEvent.Raise(this, null);
            }

            string charNameString;
            switch (currentDialog.characterName)
            {
                case CharacterName.Simon:
                case CharacterName.Mica:
                    charNameString = currentDialog.characterName.ToString();
                    break;
                case CharacterName.Cops:
                    charNameString = "POLICIA";
                    break;
                case CharacterName.Unknown:
                    charNameString = "???";
                    break;
                default:
                    charNameString = "";
                    break;
            }

            // --- Mostrar UI y Esperar ---
            yield return ShowDialogCoroutine(charNameString, textToShow, dialogTime);

            // Beep final de Simon
            if (currentDialog.characterName == CharacterName.Simon && !currentDialog.isSelfDialog)
            {
                yield return StartCoroutine(AudioManager.Instance.AudioDialogsHandler.PlayBeepSound());
            }

            // Evento de finalización
            if (currentDialog.endingEvent != null)
            {
                currentDialog.endingEvent.Raise(this, null);
            }

            // Reactivar Estática en WT
            if (shouldPauseStatic)
            {
                // Enviamos FALSE a WT: "El diálogo terminó, puedes reanudar el ruido de fondo"
                GameEvents.Common.onDialogNodeRunning.Raise(this, false);
            }

            // --- Siguiente Paso ---
            if (currentOptions != null && currentOptions.Count > 0)
            {
                ShowOptions();
            }
            else
            {
                NextDialogOrEnd();
            }
        }

        public void StartDialog(Component sender, object data)
        {
            currentDialog = (DialogSO)data;
            if (currentDialog != null)
            {
                StartCoroutine(StartDialogCoroutine(currentDialog));
            }
        }

        void ShowOptions()
        {
            optionsPanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            foreach (Transform child in optionsPanel.transform) Destroy(child.gameObject);

            for (int i = 0; i < currentOptions.Count; i++)
            {
                GameObject optionButton = Instantiate(buttonPrefab, optionsPanel.transform);

                optionButton.GetComponentInChildren<TextMeshProUGUI>().text = Utils.TextFunctions.RetrieveDialogOptions(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    currentOptions[i]
                );

                int optionIndex = i;
                Button btnComponent = optionButton.GetComponent<Button>();
                btnComponent.onClick.RemoveAllListeners();
                btnComponent.onClick.AddListener(() => OnOptionSelected(optionIndex));
            }
        }

        void NextDialogOrEnd()
        {
            if (currentDialog.nextDialog != null)
            {
                StartDialog(this, currentDialog.nextDialog);
            }
            else
            {
                // Fin del diálogo
                // Debug.Log("conversationHasEnded Raised from DialogManager");
                GameEvents.Common.onConversationHasEnded.Raise(this, null);
                dialogCanvas.enabled = false;
            }
        }

        public void OnOptionSelected(int optionIndex)
        {
            DialogSO nextDialog = currentOptions[optionIndex].nextDialog;
            if (nextDialog != null)
            {
                HideOptions();
                StartDialog(this, nextDialog);
            }
            else
            {
                HideOptions();
            }
        }

        private void HideOptions()
        {
            optionsPanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}