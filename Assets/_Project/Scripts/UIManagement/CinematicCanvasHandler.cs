namespace TwelveG.UIManagement
{
    using System.Collections;
    using UnityEngine;

    public class CinematicCanvasHandler : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Animator containerAnimator;

        private void ShowBars()
        {
            container.SetActive(true);
        }

        private IEnumerator HideBars()
        {
            if (container.activeSelf)
            {
                containerAnimator.SetTrigger("HideBars");
                yield return new WaitForSeconds(2f);
                container.SetActive(false);
            }
        }

        public void CinematicCanvasControls(Component sender, object data)
        {
            switch (data)
            {
                case ShowCinematicBars cmd:
                    if (cmd.Show)
                    {
                        ShowBars();
                    }
                    else
                    {
                        StartCoroutine(HideBars());
                    }
                    break;
                default:
                    Debug.LogWarning($"[CinematicCanvasHandler] Received unknown command: {data}");
                    break;
            }
        }
    }

}