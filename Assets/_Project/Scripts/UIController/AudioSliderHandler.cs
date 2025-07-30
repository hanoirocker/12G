namespace TwelveG.AudioController
{
  using TMPro;
  using UnityEngine;
  using UnityEngine.UI;

  public class AudioSliderHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private TextMeshProUGUI textComponent = null;

    private Slider volumeSlider = null;

    private void Awake()
    {
      volumeSlider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
      switch (volumeSlider.name)
      {
        case "Master Slider":
          volumeSlider.value = AudioManager.Instance.GetInitialChannelVolume(AudioGroup.masterVol);
          break;
        case "Music Slider":
          volumeSlider.value = AudioManager.Instance.GetInitialChannelVolume(AudioGroup.musicVol);
          break;
        case "SFX Slider":
          volumeSlider.value = AudioManager.Instance.GetInitialChannelVolume(AudioGroup.inGameVol);
          break;
        case "Interface Slider":
          volumeSlider.value = AudioManager.Instance.GetInitialChannelVolume(AudioGroup.uiVol);
          break;
        default:
          Debug.LogError($"[AudioSliderHandler]: can't recognize volumeSlider.name {volumeSlider.name}");
          break;
      }
    }

    public void SetSliderValue(float value)
    {
      // Llamar a AudioManager para cada tipo de slider
      textComponent.text = value.ToString("0.00");
    }
  }
}