namespace TwelveG.GameManager
{
  using UnityEngine;
  using System.Collections;
  using TwelveG.Localization;
  using System.Collections.Generic;
  using UnityEngine.SceneManagement;
  using TwelveG.PlayerController;
  using TwelveG.Utils;
  using TwelveG.UIManagement;

  public class NarrativeEvent : GameEventBase
  {
    [Header("Text event SO")]
    [SerializeField] private List<NarrativeTextSO> narrativeIntroTextSOs = new();

    [Header("EventsSO references")]
    public GameEventSO onShowNarrativeIntro;
    [SerializeField] private GameEventSO onControlCanvasControls;
    [SerializeField] private GameEventSO onPlayerControls;

    private bool allowNextAction = false;
    private NarrativeTextSO introTextSO;

    public override IEnumerator Execute()
    {
      print("<------ TEXT CANVAS EVENT NOW -------->");

      onPlayerControls.Raise(this, new TogglePlayerCapsule(false));

      onPlayerControls.Raise(this, new TogglePlayerShortcuts(false));

      onControlCanvasControls.Raise(this, new ActivateCanvas(false));

      onPlayerControls.Raise(this, new TogglePlayerCameraZoom(false));

      introTextSO = narrativeIntroTextSOs[SceneManager.GetActiveScene().buildIndex - 2];

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
