using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }

public class GameEventListener : MonoBehaviour
{
    // GameEventSO to listen to
    public GameEventSO gameEventSO;

    // response will determine what will happen when GameEventSO Event is raised
    // This will point at all objects in scene that will execute their own functions as reponse to the event
    public CustomGameEvent response;

    private void OnEnable()
    {
        if (gameEventSO == null)
        {
            Debug.LogWarning($"[GameEventListener]: Game Event SO reference not assigned at {gameObject.name}");
            return;
        }
        gameEventSO.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (gameEventSO == null)
        {
            Debug.LogWarning($"[GameEventListener]: Remember to assign Game Event SO at {gameObject.name}!");
            return;
        }
        gameEventSO.UnRegisterListener(this);
    }

    public void OnEventRaised(Component sender, object data)
    {
        response.Invoke(sender, data);
    }
}
