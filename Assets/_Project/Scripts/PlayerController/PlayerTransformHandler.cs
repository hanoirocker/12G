namespace TwelveG.PlayerController
{
  using UnityEngine;

  public class PlayerTransformHandler : MonoBehaviour
  {
    [Header("References")]
    [Space(5)]
    
    private Transform playerCapsuleTransform;

    public void SetPlayerTransform(Transform startingTransform)
    {
      playerCapsuleTransform = GetComponent<Transform>();

      playerCapsuleTransform.position = startingTransform.position;
      playerCapsuleTransform.rotation = startingTransform.rotation;
    }
  }
}