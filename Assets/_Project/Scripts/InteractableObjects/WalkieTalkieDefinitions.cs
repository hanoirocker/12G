using UnityEngine;
using TwelveG.DialogsController;

namespace TwelveG.InteractableObjects
{
    [System.Serializable]
    public class WalkieTalkieChannel
    {
        public int channelIndex;
        public AudioClip channelClip;
        public DialogSO pendingDialog;

        public bool HasPendingDialog() => pendingDialog != null;
        public void ClearPendingDialog() => pendingDialog = null;
    }

    [System.Serializable]
    public struct WalkieTalkieChannelData
    {
        public string channelText;
        public string frequencyText;
    }
}