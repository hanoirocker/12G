using System.Collections;
using TwelveG.DialogsController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
  public class WalkieTalkieCallHandler : MonoBehaviour
  {
    [Header("Call Settings")]
    [Tooltip("El índice del canal donde habla Micaela (Generalmente 2)")]
    [SerializeField] private int micaChannelIndex = 2;

    [Header("Game Events")]
    [SerializeField] private GameEventSO onShowDialogEvent;
    [SerializeField] private GameEventSO onShowIncomingCallPanelEvent;

    // Estado público para que el WalkieTalkie sepa si hay alguien esperando
    public bool IsIncomingCallWaiting { get; private set; } = false;

    // Propiedad pública para que el WT sepa cuál es el canal de Mica sin hardcodearlo
    public int MicaChannelIndex => micaChannelIndex;

    private DialogSO currentPendingDialog;
    private WalkieTalkieAudioHandler audioHandler;
    private System.Func<bool> checkItemShown; // Delegado para consultar estado visual

    /// Inicializa las dependencias externas
    public void Initialize(WalkieTalkieAudioHandler audioRef, System.Func<bool> isShownChecker)
    {
      this.audioHandler = audioRef;
      this.checkItemShown = isShownChecker;
    }

    /// Procesa la solicitud de diálogo y decide qué hacer.
    /// Retorna TRUE si el Walkie Talkie debe forzarse a aparecer (Caso Simon).
    public bool ProcessDialogRequest(DialogSO dialog, int currentChannel)
    {
      currentPendingDialog = dialog;
      bool isShown = checkItemShown(); // Consultamos el estado actual

      // CASO 1: MICAELA LLAMA
      if (dialog.characterName == CharacterName.Mica)
      {
        // A: Ya lo tenemos en la mano y en el canal correcto -> Atender directo
        if (isShown && currentChannel == micaChannelIndex)
        {
          AnswerCall();
          return false;
        }
        // B: Lo tenemos en la mano pero canal incorrecto -> Esperar en silencio
        else if (isShown && currentChannel != micaChannelIndex)
        {
          IsIncomingCallWaiting = true;
          return false;
        }
        // C: Guardado en el bolsillo -> Sonar alerta
        else if (!isShown)
        {
          StartCoroutine(IncomingDialogAlertCoroutine());
          return false;
        }
      }

      // CASO 2: SIMON HABLA (O llama a Mica)
      if (dialog.characterName == CharacterName.Simon)
      {
        if (dialog.isSelfDialog)
        {
          onShowDialogEvent.Raise(this, dialog);
          return false;
        }

        // Simon va a usar el radio -> Atendemos lógicamente y pedimos al WT que se muestre
        AnswerCall();
        return true;
      }

      // CASO 3: DESCONOCIDO (Ringtone)
      if (dialog.characterName == CharacterName.Unknown)
      {
        StartCoroutine(IncomingDialogAlertCoroutine());
        return false;
      }

      return false;
    }

    /// Ejecuta la acción de "Atender": Lanza el diálogo y limpia estados.
    public void AnswerCall()
    {
      IsIncomingCallWaiting = false;
      StopRinging();

      if (currentPendingDialog != null)
      {
        onShowDialogEvent.Raise(this, currentPendingDialog);
      }
    }

    /// Usado cuando el jugador cambia manualmente al canal correcto
    public void AcceptWaitingCall()
    {
      if (IsIncomingCallWaiting)
      {
        AnswerCall();
      }
    }

    public void ResumeRingingIfWaiting()
    {
      if (IsIncomingCallWaiting)
      {
        StartCoroutine(IncomingDialogAlertCoroutine());
      }
    }

    private void StopRinging()
    {
      // Solo detenemos el audio y el panel, pero NO cambiamos IsIncomingCallWaiting a false aquí
      // (eso solo se hace al atender).
      audioHandler.Stop();
      onShowIncomingCallPanelEvent.Raise(this, false);
    }

    private IEnumerator IncomingDialogAlertCoroutine()
    {
      IsIncomingCallWaiting = true;
      onShowIncomingCallPanelEvent.Raise(this, true);

      audioHandler.PlayIncomingAlert();

      // Esperamos hasta que el jugador saque el objeto
      yield return new WaitUntil(() => checkItemShown());

      // Al sacar el objeto, el ringtone se corta visualmente
      onShowIncomingCallPanelEvent.Raise(this, false);
      audioHandler.Stop();
    }
  }
}