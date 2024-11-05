namespace TwelveG.Utils
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class TextFunctions
    {
        public static float CalculateTextDisplayDuration(string text)
        {
            float reason = 8f;
            float calculatedTime = (text.Length) / reason;
            return calculatedTime;
        }
    }
}