namespace TwelveG.Environment
{
    using System;
    using System.Collections.Generic;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class ContemplateObject : MonoBehaviour, IContemplable
    {
        [Header("Contemplation Settings")]
        public bool hasReachedMaxContemplations = false;

        private bool ableToInteract;
        private string contemplationText;
        private int defaultContemplationTextsCounter = 0;

        private void Start()
        {
            ableToInteract = true;
        }

        public string GetContemplationText()
        {
            return "TODO: LOCALIZATION!!!!";
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