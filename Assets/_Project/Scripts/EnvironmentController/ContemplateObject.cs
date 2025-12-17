using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.EnvironmentController
{
    public class ContemplateObject : MonoBehaviour, IContemplable
    {
        [Header("Contemplation Settings")]
        public bool repeatTexts = false;
        public bool hasReachedMaxContemplations = false;
        [SerializeField] private ContextualContemplationSO contextualSO;

        private int currentContemplationIndex = 0;
        private bool ableToInteract;
        private int defaultContemplationTextsCounter = 0;

        private void Start()
        {
            ableToInteract = true;
        }

        public string GetContemplationText()
        {
            SceneEnum sceneEnum = SceneUtils.RetrieveCurrentSceneEnum();
            int eventIndex = GameManager.Instance.EventsHandler.RetrieveCurrentEventIndex();

            var so = contextualSO.GetSOForContext(sceneEnum, eventIndex);
            if (so == null) return string.Empty;

            var (contemplationText, updatedContemplationsStatus, updatedIndex) =
                Utils.TextFunctions.RetrieveContemplationText(
                    currentContemplationIndex,
                    repeatTexts,
                    so);

            hasReachedMaxContemplations = updatedContemplationsStatus;
            currentContemplationIndex = updatedIndex;
            return contemplationText;
        }

        public bool CanBeInteractedWith()
        {
            return ableToInteract;
        }

        public void IsAbleToBeContemplate(bool isAble)
        {
            ableToInteract = isAble;
        }

        public bool HasReachedMaxContemplations()
        {
            return hasReachedMaxContemplations;
        }

        public int RetrieveDefaultTextCounter()
        {
            return defaultContemplationTextsCounter;
        }

        public void UpdateDefaultTextCounter()
        {
            defaultContemplationTextsCounter += 1;
        }
    }

}