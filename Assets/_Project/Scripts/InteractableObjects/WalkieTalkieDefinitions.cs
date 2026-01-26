using UnityEngine;
using TwelveG.DialogsController;

namespace TwelveG.InteractableObjects
{
    [System.Serializable]
    public class WalkieTalkieChannel
    {
        public int channelIndex;

        // Datos del SO
        public AudioClip staticClip; // Ruido de fondo
        public AudioClip loreClip;   // Clip especial del evento en dicho canal
        public DialogSO reactionDialog; // Reacción de Simon

        // Datos Runtime
        public DialogSO pendingDialog; // Para la policía (LoadDialogForSpecificChannel)
        public bool hasPlayedLore;     // Flag para saber si ya escuchamos el lore de este evento

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