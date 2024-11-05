namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ItemControlText", menuName = "SO's/ControlTextSO", order = 0)]
    public class ItemControlTextSO : ScriptableObject
    {
        [TextArea(3, 10)]
        public string text;
    }

}