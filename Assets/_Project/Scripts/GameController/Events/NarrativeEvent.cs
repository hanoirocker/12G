using UnityEngine;
using System.Collections;
using TwelveG.Localization;
using System.Collections.Generic;
using TwelveG.PlayerController;
using TwelveG.UIController;
using UnityEngine.SceneManagement;

namespace TwelveG.GameController
{
  public class NarrativeEvent : GameEventBase
  {
    [Header("Text event SO")]
    public List<NarrativeTextSO> narrativeIntroTextSOs = new List<NarrativeTextSO>();

    private bool allowNextAction = false;
    private NarrativeTextSO introTextSO;

    public override IEnumerator Execute()
    {

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));

      GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));

      introTextSO = narrativeIntroTextSOs[SceneManager.GetActiveScene().buildIndex - 2];

      GameEvents.Common.onShowNarrativeIntro.Raise(
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
      allowNextAction = true;
    }

    public void ResetAllowNextActions()
    {
      allowNextAction = false;
    }
  }
}
