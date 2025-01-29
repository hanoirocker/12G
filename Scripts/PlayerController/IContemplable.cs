namespace TwelveG.PlayerController
{
    interface IContemplable
    {
        public bool HasReachedMaxContemplations();

        public string GetContemplationText(string languageCode);

        public bool CanBeInteractedWith();

        public void IsAbleToBeContemplate(bool isAble);

        public int RetrieveDefaultTextCounter();

        public void UpdateDefaultTextCounter();
    }
}