namespace TwelveG.PlayerController
{
  using TwelveG.GameController;
  using UnityEngine;

  public class PlayerTransformHandler : MonoBehaviour
  {
    [Header("References")]
    [Space(5)]
    [SerializeField] private Transform[] playerTransforms;
    
    private Transform playerCapsuleTransform;

    public void SetPlayerTransform(EventsEnum eventEnum)
    {
      playerCapsuleTransform = GetComponent<Transform>();

      for (int i = 0; i < playerTransforms.Length; i++)
      {
        if (playerTransforms[i].gameObject.name == eventEnum.ToString())
        {
          playerCapsuleTransform.localPosition = playerTransforms[i].localPosition;
          playerCapsuleTransform.localRotation = playerTransforms[i].localRotation;
        }
      }
    }
  }
}