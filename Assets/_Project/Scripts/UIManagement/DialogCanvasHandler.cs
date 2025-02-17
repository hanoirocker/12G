namespace TwelveG.UIManagement
{
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using TwelveG.Utils;

    public class DialogCanvasHandler : MonoBehaviour
    {   
        public static bool canvasIsShowing;

        private Canvas dialogCanvas;
        private TextMeshProUGUI dialogCanvasText;

        private void Awake()
        {
            dialogCanvas = GetComponent<Canvas>();
            dialogCanvasText = GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start()
        {
            dialogCanvas.enabled = false;
            canvasIsShowing = dialogCanvas.enabled;
        }

        public void ShowDialog(Component sender, object data)
        {
            if ((string)data != null)
            {
                StartCoroutine(ShowDialogCoroutine((string)data));
            }
        }

        private IEnumerator ShowDialogCoroutine(string data)
        {
            dialogCanvasText.text = data;
            dialogCanvas.enabled = true;
            canvasIsShowing = true;
            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(data));
            dialogCanvas.enabled = false;
            canvasIsShowing = false;
        }
    }
}