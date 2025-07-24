namespace TwelveG.EnvironmentController
{
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class ContemplateObject : MonoBehaviour, IContemplable
    {
        [Header("Contemplation Settings")]
        public bool hasReachedMaxContemplations = false;
        [SerializeField] private ContemplationTextSO contemplationTextsSO;

        private int currentContemplationIndex = 0;
        private bool ableToInteract;
        private string contemplationText;
        private int defaultContemplationTextsCounter = 0;

        private void Start()
        {
            ableToInteract = true;
        }

        public string GetContemplationText(string languageCode)
        {
            var (contemplationText, updatedContemplationsStatus, updatedIndex) = 
                Utils.TextFunctions.RetrieveContemplationText(
                    languageCode,
                    currentContemplationIndex,
                    contemplationTextsSO);

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