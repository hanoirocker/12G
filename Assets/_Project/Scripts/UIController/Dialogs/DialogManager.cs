using System.Collections;
using TMPro;
using TwelveG.AudioController;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.DialogsController
{
    public class DialogManager : MonoBehaviour
    {
        public GameObject optionsPanel;
        public GameObject buttonPrefab;
        public GameEventSO conversationHasEnded;

        private DialogSO currentDialog;

        [SerializeField] TextMeshProUGUI dialogCanvasText;
        [SerializeField] TextMeshProUGUI characterNameCanvasText;

        private void Start()
        {
            dialogCanvasText.gameObject.SetActive(false);
            characterNameCanvasText.gameObject.SetActive(false);
        }

        public void HideDialog()
        {
            dialogCanvasText.gameObject.SetActive(false);
            characterNameCanvasText.gameObject.SetActive(false);
        }

        public bool DialogIsShowing()
        {
            return dialogCanvasText.gameObject.activeSelf;
        }

        public IEnumerator ShowDialogCoroutine(string charName, string dialog, float audioClipDuration)
        {
            characterNameCanvasText.text = charName + ":";
            dialogCanvasText.text = dialog;

            dialogCanvasText.gameObject.SetActive(true);
            characterNameCanvasText.gameObject.SetActive(true);

            yield return new WaitForSeconds(audioClipDuration);

            dialogCanvasText.gameObject.SetActive(false);
            characterNameCanvasText.gameObject.SetActive(false);
        }

        private IEnumerator StartDialogCoroutine(DialogSO currentDialog)
        {
            float timeBeforeShowing = currentDialog.timeBeforeShowing;

            yield return new WaitForSeconds(timeBeforeShowing);

            AudioManager.Instance.AudioDialogsHandler.PlayDialogClip(currentDialog.spanishDialogClip);
            yield return ShowDialogCoroutine(currentDialog.characterName.ToString(), currentDialog.dialogText, currentDialog.spanishDialogClip.length);

            // Verifica si el diálogo tiene opciones
            if (currentDialog.options != null && currentDialog.options.Length > 0)
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
            if (data != null)
            {
                currentDialog = (DialogSO)data;

                StartCoroutine(StartDialogCoroutine(currentDialog));
            }
        }

        void ShowOptions()
        {
            optionsPanel.SetActive(true);
            Cursor.visible = true;  // Muestra el cursor
            Cursor.lockState = CursorLockMode.None;  // Desbloquea el cursor

            // Elimina cualquier botón existente
            foreach (Transform child in optionsPanel.transform)
            {
                Destroy(child.gameObject);
            }

            // Instancia botones basados en la cantidad de opciones
            for (int i = 0; i < currentDialog.options.Length; i++)
            {
                GameObject optionButton = Instantiate(buttonPrefab, optionsPanel.transform);
                optionButton.GetComponentInChildren<TextMeshProUGUI>().text = currentDialog.options[i].optionText;
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
                StartDialog(this, currentDialog.nextDialog); // Continúa al siguiente diálogo
            }
            else
            {
                // Fin del diálogo
                print("No hay mas dialogos para mostrar, invocando conversationHasEnded");
                conversationHasEnded.Raise(this, null);
                optionsPanel.SetActive(false);
            }
        }

        public void OnOptionSelected(int optionIndex)
        {
            DialogSO nextDialog = currentDialog.options[optionIndex].nextDialog;
            if (nextDialog != null)
            {
                HideOptions();
                StartDialog(this, nextDialog);
            }
            else
            {
                // Fin del diálogo o acción adicional
                HideOptions();
            }
        }

        private void HideOptions()
        {
            optionsPanel.SetActive(false);
            Cursor.visible = false;  // Oculta el cursor
            Cursor.lockState = CursorLockMode.Locked;  // Bloquea el cursor
        }
    }
}