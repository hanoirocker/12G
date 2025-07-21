namespace TwelveG.GameManager
{
  using UnityEngine;
  using System.Collections;
  using TwelveG.Localization;
  using System.Collections.Generic;
  using UnityEngine.SceneManagement;

  public class NarrativeEvent : GameEventBase
  {
    [Header("Text event SO")]
    [SerializeField] private List<NarrativeTextSO> narrativeIntroTextSOs = new();

    [Header("EventsSO references")]
    public GameEventSO onShowNarrativeIntro;

    private bool allowNextAction = false;
    private NarrativeTextSO introTextSO;

    public override IEnumerator Execute()
    {
      print("<------ TEXT CANVAS EVENT NOW -------->");
      introTextSO = narrativeIntroTextSOs[SceneManager.GetActiveScene().buildIndex];

      onShowNarrativeIntro.Raise(
          this,
          introTextSO
      );

      // Unity Event (NarrativeCanvasHandler - allowNextAction):
      // Se recibe cuando termina la corrutina del canvas
      yield return new WaitUntil(() => allowNextAction);
      
      yield return new WaitForSeconds(3f);
    }

    public void AllowNextActions(Component sender, object data)
    {
      print(gameObject.name + "recibió eventoSO desde " + sender.gameObject.name + " para continuar el evento");
      allowNextAction = true;
    }

    public void ResetAllowNextActions()
    {
      allowNextAction = false;
    }
  }
}
