namespace TwelveG.InteractableObjects
{
    using UnityEngine;

    public class RemoteControlHandler : MonoBehaviour
    {
        [Header("EventsSO references")]
        public GameEventSO setRemoteControl;

        public void SetRemoteControl()
        {
            setRemoteControl.Raise(this, gameObject);
        }
    }
}